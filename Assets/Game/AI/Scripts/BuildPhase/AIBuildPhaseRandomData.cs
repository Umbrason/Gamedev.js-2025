using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "AIBuildPhaseRandomData", menuName = "Scriptable Objects/AI/BuildingPhase/Random")]
public class AIBuildPhaseRandomData : AIBuildPhaseData
{
    [SerializeField, Min(0)] int maxBuildingPerTurn = 1;
    public override IEnumerator PlayingPhase(BuildPhase Phase, AIPlayer AI)
    {
        yield return new WaitForSeconds(AI.Config.ActionDelay);

        //BUILDING
        for (int i = 0; i < maxBuildingPerTurn; i++)
        {
            List<Building> affordableBuildings = GetAffordableBuildings(AI.GameInstance);

            if (affordableBuildings.Count == 0) break;

            Building building = affordableBuildings.GetRandom();

            if (!building.FindBestPosition(AI.GameInstance.ClientPlayerData.Island, out HexPosition pos, out _)) continue;

            AI.Log("builds " + building);
            Phase.PlaceBuilding(pos, building);

            yield return new WaitForSeconds(AI.Config.ActionDelay);
        }

        //PLEDGING
        if (AI.GameInstance.ClientPlayerData.Role == PlayerRole.Selfish)
        {
            PledgeCommonGoals(PlayerRole.Selfish, Phase, AI);
        }
        PledgeCommonGoals(PlayerRole.Balanced, Phase, AI);
        yield return new WaitForSeconds(AI.Config.ActionDelay);


        Phase.Skip();
    }

    private void PledgeCommonGoals(PlayerRole role, BuildPhase Phase, AIPlayer AI)
    {
        if (role == PlayerRole.None) return;

        var Goals = role == PlayerRole.Selfish ? AI.GameInstance.SelfishFactionGoals :  AI.GameInstance.BalancedFactionGoals;

        for (int i = 0; i < Goals.Count; i++)
        {
            SharedGoal goal = Goals[i];

            foreach ((Resource res, int required) in goal.Required)
            {
                int stock = AI.GameInstance.ClientPlayerData.Resources.GetValueOrDefault(res);
                int pledge = UnityEngine.Random.Range(0, Mathf.Min(stock, required) + 1);

                if (pledge == 0) continue;
                
                AI.Log(string.Format("offers {0} {1} to {2}", pledge, res, goal.Name));
                Phase.PledgeResource(new(role, i), res, pledge);
            }
        }
    }
}
