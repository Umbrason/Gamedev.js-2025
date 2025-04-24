using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public abstract class AIBuildPhaseData : AIPhaseData<BuildPhase>
{
    static protected List<Building> GetAffordableBuildings(BuildPhase Phase)
    {
        List<Building> affordableBuildings = new();

        foreach (Building building in Enum.GetValues(typeof(Building)))
        {
            if (building == Building.None) continue;
            if (Phase.CanAffordBuilding(building))
                affordableBuildings.Add(building);
        }

        return affordableBuildings;
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
