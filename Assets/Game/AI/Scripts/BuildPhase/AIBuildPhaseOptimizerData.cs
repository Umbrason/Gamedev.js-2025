using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Playables;

[CreateAssetMenu(fileName = "AIBuildPhaseOptimizerData", menuName = "Scriptable Objects/AIBuildPhaseOptimizerData")]
public class AIBuildPhaseOptimizerData : AIBuildPhaseData
{
    [Header("Building")]
    [SerializeField] AIUtilityEstimator ResourcesUtilitiesEstimator;


    [Header("Balanced AI Player offerings")]

    [SerializeField]
    [Tooltip("min-max range of the proportion of owned resources that will be offered")]
    Vector2 BaseOfferingProportion = new Vector2(0f,0.1f);

    [SerializeField]
    Vector2 OfferingIncreaseByTurn = new Vector2(0.05f,0.05f);

    [Header("Selfish AI Player offerings to balanced goals")]

    [SerializeField]
    [Tooltip("min-max range of the proportion of owned resources that will be offered")]
    Vector2 BaseSelfishToBalanceOfferingProportion = new Vector2(0f, 0.1f);

    [SerializeField]
    Vector2 SelfishToBalanceOfferingIncreaseByTurn = new Vector2(0.05f, 0.05f);

    [Header("Selfish AI Player offerings to selfish goals")]

    [SerializeField]
    [Tooltip("min-max range of the proportion of owned resources that will be offered")]
    Vector2 BaseSelfishOfferingProportion = new Vector2(0f, 0.1f);

    [SerializeField]
    Vector2 SelfishOfferingIncreaseByTurn = new Vector2(0.05f, 0.05f);

    public override IEnumerator PlayingPhase(BuildPhase Phase, AIPlayer AI)
    {
        yield return new WaitForSeconds(AI.Config.ActionDelay);

        #region BUILDING
        ResourcesUtilities ResourcesUtilities = ResourcesUtilitiesEstimator.GetResourcesUtilities(AI.GameInstance);

        BuildingUtility BestBuilding;

        while(true)
        {
            BestBuilding = null;

            foreach (Building building in GetAffordableBuildings(Phase))
            {
                BuildingUtility Building = new BuildingUtility(building, ResourcesUtilities, AI.GameInstance);
                if (Building.Utility <= 0f) continue;//Dont build useless buildings
                if (BestBuilding == null || BestBuilding.Utility < Building.Utility)
                {
                    BestBuilding = Building;
                }
            }

            if(BestBuilding == null) break;

            AI.Log("builds " + BestBuilding.Building);
            Phase.PlaceBuilding(BestBuilding.Position, BestBuilding.Building);

            yield return new WaitForSeconds(AI.Config.ActionDelay);

        }

        #endregion

        #region PLEDGING
        float balancedProp, selfishProp;

        if (AI.GameInstance.ClientPlayerData.Role == PlayerRole.Selfish)
        {
            selfishProp = RandomProportion(BaseSelfishOfferingProportion, SelfishOfferingIncreaseByTurn);
            balancedProp = RandomProportion(BaseSelfishToBalanceOfferingProportion, SelfishToBalanceOfferingIncreaseByTurn);
        }
        else
        {
            selfishProp = 0f;
            balancedProp = RandomProportion(BaseOfferingProportion, OfferingIncreaseByTurn);
        }

        //normalise 
        float totProp = selfishProp + balancedProp;
        if (totProp > 1f)
        {
            selfishProp /= totProp;
            balancedProp /= totProp;
        }

        //as balanced goal is paid after selfish, adjust its proportion
        //example: 10% balance, 70% selfish, 10 resources
        //7 res are offered to selfish
        //33.33% of the remaining 3 resources should be offered to balance
        balancedProp = balancedProp / (1f - selfishProp);

        PledgeCommonGoals(PlayerRole.Selfish, Phase, AI, selfishProp);
        PledgeCommonGoals(PlayerRole.Balanced, Phase, AI, balancedProp);

        #endregion

        yield return new WaitForSeconds(AI.Config.ActionDelay);


        Phase.Skip();

        float RandomProportion(Vector2 rangeBase, Vector2 rangeIncrease)
        {
            Vector2 range = rangeBase + (AI.GameInstance.Turn - 1f) * rangeIncrease;
            float prop = UnityEngine.Random.Range(range.x, range.y);
            return Mathf.Max(0f, prop);
        }
    }

    private void PledgeCommonGoals(PlayerRole faction, BuildPhase Phase, AIPlayer AI, float resourcesProportion)
    {
        if (faction == PlayerRole.None) return;
        if(resourcesProportion <= 0f) return;   

        if(resourcesProportion > 1f) resourcesProportion = 1f;

        var Goals = faction == PlayerRole.Selfish ? AI.GameInstance.SelfishFactionGoals : AI.GameInstance.BalancedFactionGoals;

        Dictionary<Resource, int> MaxOffering = new();
        foreach((Resource res, int quantity) in AI.GameInstance.ClientPlayerData.Resources)
            MaxOffering[res] = Mathf.RoundToInt(quantity * resourcesProportion);


        for (int i = 0; i < Goals.Count; i++)
        {
            SharedGoal goal = Goals[i];

            foreach ((Resource res, int required) in goal.Required)
            {
                int pledge = Mathf.Min(MaxOffering.GetValueOrDefault(res), required);

                if (pledge == 0) continue;

                MaxOffering[res] -= pledge;

                AI.Log(string.Format("offers {0} {1} to {2}", pledge, res, goal.Name));
                Phase.PledgeResource(new(faction, i), res, pledge);
            }
        }
    }

    private class BuildingUtility
    {
        public Building Building { get; private set; }
        public float Utility { get; private set; }
        public HexPosition Position { get; private set; }

        public BuildingUtility(Building building, ResourcesUtilities ResourcesUtilities, GameInstance Game)
        {
            Building = building;
            Utility = -1000f;
            Position = default;

            if (!building.FindBestPosition(Game.ClientPlayerData.Island, out HexPosition pos, out float yield)) return;
            
            Position = pos;
            Utility = 0f;

            foreach ((Resource res, int quantity) in building.ConstructionCosts())
            {
                Utility -= ResourcesUtilities.Stock[res] * quantity;
            }
            foreach((Resource res, int quantity) in building.OperationCosts())
            {
                Utility -= ResourcesUtilities.Prodution[res] * quantity;
            }
            Utility += ResourcesUtilities.Prodution[building.ResourceYieldType()] * yield;
        }

    }
}
