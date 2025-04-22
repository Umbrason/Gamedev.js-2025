using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class AIPetitionPhaseData : AIPhaseData<PetitionPhase>
{
    static protected List<PlayerID> GetPlayerIDs(GameInstance game, PlayerID exept = PlayerID.None)
    {
        List<PlayerID> ids = new();
        foreach (PlayerID id in game.PlayerData.Keys)
        {
            if(id != exept) ids.Add(id);
        }
        return ids;
    }

    static protected HexPosition[] GetFreeTiles(PlayerIsland island)
    {
        HashSet<HexPosition> tilesPositions = island.Tiles.Keys.ToHashSet();
        HashSet<HexPosition> buildingPositions = island.Buildings.Keys.ToHashSet();

        tilesPositions.ExceptWith(buildingPositions);

        return tilesPositions.ToArray();
    }

}
