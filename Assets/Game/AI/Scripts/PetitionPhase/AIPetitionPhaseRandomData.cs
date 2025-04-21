using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "AIPetitonPhaseRandomData", menuName = "Scriptable Objects/AI/Petition/Random")]
public class AIPetitionPhaseRandomData : AIPetitionPhaseData
{

    public override IEnumerator PlayingPhase(PetitionPhase Phase, AIConfigData Config, GameInstance GameInstance)
    {
        yield return new WaitForSeconds(Config.ActionDelay);

        var otherPlayersIds = GetPlayerIDs(GameInstance, exept: GameInstance.ClientID);

        PlayerID builderPlayerId = otherPlayersIds.GetRandom();
        PlayerData builderPlayerData = GameInstance.PlayerData[builderPlayerId];

        PlayerID targetIslandPlayerId = GetPlayerIDs(GameInstance).GetRandom();
        PlayerIsland targetIsland = GameInstance.PlayerData[targetIslandPlayerId].Island;

        Building building = System.Enum.GetValues(typeof(Building)).Cast<Building>().ToArray().GetRandom();

        HexPosition targetPosition = GetFreeTiles(targetIsland).GetRandom();

        BuildingPetition petition = new BuildingPetition(GameInstance.ClientID, targetPosition, building, new() { { builderPlayerId, BuildingExtensions.ConstructionCosts(building) } });

        Phase.SubmitPetition(petition);
    }
}
