using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AIPetitonPhaseRandomData", menuName = "Scriptable Objects/AI/Petition/Random")]
public class AIPetitionPhaseRandomData : AIPetitionPhaseData
{

    public override IEnumerator PlayingPhase(PetitionPhase Phase, AIPlayer AI)
    {
        yield return new WaitForSeconds(AI.Config.ActionDelay);

        var otherPlayersIds = GetPlayerIDs(AI.GameInstance, exept: AI.GameInstance.ClientID);

        PlayerID builderPlayerId = otherPlayersIds.GetRandom();
        PlayerData builderPlayerData = AI.GameInstance.PlayerData[builderPlayerId];

        PlayerID targetIslandPlayerId = GetPlayerIDs(AI.GameInstance).GetRandom();
        PlayerIsland targetIsland = AI.GameInstance.PlayerData[targetIslandPlayerId].Island;

        Building building = System.Enum.GetValues(typeof(Building)).Cast<Building>().ToArray().GetRandom();

        HexPosition targetPosition = GetFreeTiles(targetIsland).GetRandom();

        AI.Log(string.Format("petitions for a {0} in island {1}", building, targetIslandPlayerId));

        BuildingPetition petition = new BuildingPetition(AI.GameInstance.ClientID, targetPosition, building, new() { { builderPlayerId, BuildingExtensions.ConstructionCosts(building) } });

        Phase.SubmitPetition(petition);
    }
}
