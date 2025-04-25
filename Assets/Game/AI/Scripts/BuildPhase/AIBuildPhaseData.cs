using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public abstract class AIBuildPhaseData : AIPhaseData<BuildPhase>
{
    static public List<Building> GetAffordableBuildings(GameInstance Game)
    {
        List<Building> affordableBuildings = new();

        foreach (Building building in Enum.GetValues(typeof(Building)))
        {
            if (building == Building.None) continue;
            if (CanAffordBuilding(building))
                affordableBuildings.Add(building);
        }

        return affordableBuildings;

        bool CanAffordBuilding(Building building) => Game.ClientPlayerData.HasResources(building.ConstructionCosts());
        
    }

    static protected List<HexPosition> GetBuildablePositions(BuildPhase Phase, Building Building, GameInstance GameInstance)
    {
        List<HexPosition> positions = new();

        foreach(HexPosition pos in GameInstance.ClientPlayerData.Island.Tiles.Keys.OrderBy(x=>UnityEngine.Random.value))
        {
            if(Phase.CanPlaceBuilding(pos, Building))
                positions.Add(pos);
        }

        return positions;
    }
}
