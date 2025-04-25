using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourcesUtilityEstimator", menuName = "Scriptable Objects/AI/ResourcesUtilityEstimator2")]
public class AIUtilityEstimator2 : AIUtilityEstimator
{
    [Header("Production impact (avoid producing too much and spend more when producing a lot)")]

    [Tooltip("Reduces exponentialy utility for each unit produced each turn")]
    [Range(0f, 1f)]
    [SerializeField]
    float basicResourceProductionImpact = 0.1f;

    [Tooltip("Reduces exponentialy utility for each unit produced each turn")]
    [Range(0f, 1f)]
    [SerializeField]
    float combinedResourceProductionImpact = 0.1f;



    [Header("Goals progress impact on required resources utility")]

    [Tooltip("The production utility of basic resources is multiplied by this value. It lerps between x and y, with t the fraction of this resource that has already been paid. t is always 1 for resources that are not needed")]
    [SerializeField]
    Vector2 GoalProgressFactorBasicResources = new Vector2(1f, 1f);

    [Tooltip("The production utility of combined resources is multiplied by this value. It lerps between x and y, with t the fraction of this resource that has already been paid. t is always 1 for resources that are not needed")]
    [SerializeField]
    Vector2 GoalProgressFactorCombinedResources = new Vector2(1f, 1f);

    [Tooltip("at 1 the selfish AI only considers the selfish goal, at 0 only the balance goal")]
    [Range(0f, 1f)][SerializeField] 
    float selfishGoalWeight = 0.8f;

    [Header("Utility randomization")]
    [Tooltip("Increase to make AI more random")]
    [SerializeField, Range(0f, 1f)] float noiseStrength = 0.1f;

    private float NoisyFactor
    {
        get
        {
            float noiseValue = 1f - noiseStrength * UnityEngine.Random.value;

            return UnityEngine.Random.value < 0.5f ? noiseValue : 1f / noiseValue;
        }
    }

    override public ResourcesUtilities GetResourcesUtilities(GameInstance Game)
    {
        ResourcesUtilities utilities = base.GetResourcesUtilities(Game);

        FactorProduction(utilities);
        FactorGoals(utilities);

        return utilities;

        void FactorProduction(ResourcesUtilities utilities){
            foreach((Resource res, float prod) in Game.ClientPlayerData.Island.GetAverageProduction())
            {
                float productionImpact = res.IsBasic() ? basicResourceProductionImpact : combinedResourceProductionImpact;

                utilities.Prodution[res] *= Mathf.Pow(1f - productionImpact, prod);
            }
        }

        void FactorGoals(ResourcesUtilities utilities)
        {
            Dictionary<Resource, float> GoalsProgress = GetGoalsProgres(Game, PlayerRole.Balanced);

            if(Game.ClientPlayerData.Role == PlayerRole.Selfish)
            {
                Dictionary<Resource, float> SelfishGoalsProgress = GetGoalsProgres(Game, PlayerRole.Selfish);

                foreach(Resource res in GoalsProgress.Keys.ToList())
                {
                    GoalsProgress[res] = Mathf.Lerp(GoalsProgress[res], SelfishGoalsProgress[res], selfishGoalWeight);
                }
            }

            foreach (Resource res in Enum.GetValues(typeof(Resource)))
            {
                Vector2 progressFactorRange = res.IsBasic() ? GoalProgressFactorBasicResources : GoalProgressFactorCombinedResources;
                float progressFactor = Mathf.Lerp(progressFactorRange.x, progressFactorRange.y, GoalsProgress[res]);

                if(utilities.Prodution.ContainsKey(res)) utilities.Prodution[res] *= progressFactor;

                //Only lower production because lowering stock utility will make the AI "waste" resources instead of saving them for the goals.
                //if (utilities.Stock.ContainsKey(res)) utilities.Stock[res] *= progressFactor;
            }
        }

        Dictionary<Resource, float> GetGoalsProgres(GameInstance Game, PlayerRole role)
        {
            var goals = role == PlayerRole.Selfish ? Game.SelfishFactionGoals : Game.BalancedFactionGoals;

            Dictionary<Resource, int> required = new();
            Dictionary<Resource, int> collected = new();

            foreach(SharedGoal goal in goals)
            {
                foreach((Resource res, int quantity) in goal.Required)
                {
                    required[res] = required.GetValueOrDefault(res) + quantity;
                }
                foreach ((Resource res, int quantity) in goal.Collected)
                {
                    collected[res] = collected.GetValueOrDefault(res) + quantity;
                }
            }

            Dictionary<Resource, float> progress = new();

            foreach(Resource res in Enum.GetValues(typeof(Resource)))
            {
                int r = required.GetValueOrDefault(res);
                int c = collected.GetValueOrDefault(res);

                progress[res] = r <= 0f ? 1f : (float)c / (float)r;                
            }

            return progress;
        }
    }

    public override BuildingUtility GetBestBuilding(GameInstance Game, ResourcesUtilities ResourcesUtilities, bool onlyAffordable = true)
    {            
        BuildingAmount SecretTask = Game.ClientPlayerData.SecretGoal as BuildingAmount;
        float secretTaskUtility = SecretTask != null ? GetAllBuildingsProductionUtility() * BuildPhase.SecretTaskRewardResourceMultiplier : 0f;


        BuildingUtility BestBuilding = null;

        List<Building> Buildings = new();
        if (onlyAffordable) Buildings = AIBuildPhaseData.GetAffordableBuildings(Game);
        else
        {
            foreach (Building building in Enum.GetValues(typeof(Building)))
            {
                if (building != Building.None) Buildings.Add(building);
            }
        }

        foreach (Building building in Buildings)
        {
            BuildingUtility Building = GetBuildingUtility(Game, ResourcesUtilities, SecretTask, secretTaskUtility, building);

            if (Building.Utility <= 0f) continue;//Dont build useless buildings


            if (BestBuilding == null || BestBuilding.Utility < Building.Utility)
            {
                BestBuilding = Building;
            }
        }

        return BestBuilding;


        float GetAllBuildingsProductionUtility()
        {
            float utility = 0f;
            foreach (var (position, building) in Game.ClientPlayerData.Island.Buildings)
            {
                float yield = building.ExpectedYield(Game.ClientPlayerData.Island, position);
                Resource res = building.ResourceYieldType();

                utility += ResourcesUtilities.Prodution[res] * yield;
            }
            return utility;
        }
    }

    private BuildingUtility GetBuildingUtility(GameInstance Game, ResourcesUtilities ResourcesUtilities, BuildingAmount SecretTask, float secretTaskUtility, Building building)
    {
        BuildingUtility Building = new BuildingUtility(building, ResourcesUtilities, Game);

        if (SecretTask != null)
        {
            Building.Utility += secretTaskUtility * SecretTask.ProgressIfBuilt(building);
        }

        Building.Utility *= NoisyFactor;

        return Building;
    }
}
