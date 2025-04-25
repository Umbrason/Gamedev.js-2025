using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "ResourcesUtilityEstimator", menuName = "Scriptable Objects/AI/ResourcesUtilityEstimator")]
public partial class AIUtilityEstimator : ScriptableObject
{
    [SerializeField, Min(0f)] private float basicResourcesBaseUtility = 1f; 
    [SerializeField, Min(0f)] private float combinedResourcesBaseUtility = 3f; 


    [SerializeField, Min(0f), Tooltip("How much is valued increasing the production by one")]
    private float basicResourcesBaseProductionUtility = 10f; 

    [SerializeField, Min(0f), Tooltip("How much is valued increasing the production by one")]
    private float combinedResourcesBaseProductionUtility = 30f; 

    public virtual ResourcesUtilities GetResourcesUtilities(GameInstance Game)
    {
        ResourcesUtilities values = new();

        foreach(Resource res in Enum.GetValues(typeof(Resource)))
        {
            bool basic = (int)res <= 6;

            values.Stock[res] = basic ? basicResourcesBaseUtility : combinedResourcesBaseUtility;

            values.Prodution[res] = basic ? basicResourcesBaseProductionUtility : combinedResourcesBaseProductionUtility;
        }

        return values;
    }

    public virtual BuildingUtility GetBestBuilding(GameInstance Game, ResourcesUtilities ResourcesUtilities, bool onlyAffordable = true)
    {
        BuildingUtility BestBuilding = null;

        List<Building> Buildings = new();
        if (onlyAffordable) Buildings = AIBuildPhaseData.GetAffordableBuildings(Game);
        else
        {
            foreach(Building building in Enum.GetValues(typeof(Building))){
                if(building != Building.None) Buildings.Add(building);
            }
        }

        foreach (Building building in Buildings)
        {
            BuildingUtility Building = new BuildingUtility(building, ResourcesUtilities, Game);
            if (Building.Utility <= 0f) continue;//Dont build useless buildings
            if (BestBuilding == null || BestBuilding.Utility < Building.Utility)
            {
                BestBuilding = Building;
            }
        }

        return BestBuilding;
    }

    public class BuildingUtility
    {
        public Building Building { get; private set; }
        public float Utility { get; set; }
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
            foreach ((Resource res, int quantity) in building.OperationCosts())
            {
                Utility -= ResourcesUtilities.Prodution[res] * quantity;
            }
            Utility += ResourcesUtilities.Prodution[building.ResourceYieldType()] * yield;
        }

    }



}
