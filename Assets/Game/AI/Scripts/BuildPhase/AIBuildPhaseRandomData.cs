using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AIBuildPhaseRandomData", menuName = "Scriptable Objects/AI/BuildingPhase/Random")]
public class AIBuildPhaseRandomData : AIBuildPhaseData
{
    [SerializeField, Min(0)] int maxBuildingPerTurn = 1;
    public override IEnumerator PlayingPhase(BuildPhase Phase, AIConfigData Config, GameInstance GameInstance)
    {
        yield return new WaitForSeconds(Config.ActionDelay);

        for (int i = 0; i < maxBuildingPerTurn; i++)
        {
            List<Building> affordableBuildings = GetAffordableBuildings(Phase);

            if (affordableBuildings.Count == 0) break;

            Building building = affordableBuildings[UnityEngine.Random.Range(0, affordableBuildings.Count)];

            List<HexPosition> positions = GetBuildablePositions(Phase, building, GameInstance);

            HexPosition pos = positions[UnityEngine.Random.Range(0, positions.Count)];

            Phase.PlaceBuilding(pos, building);

            yield return new WaitForSeconds(Config.ActionDelay);
        }

        Phase.Skip();
    }
}
