using System.Collections.Generic;
using UnityEngine;

public class BuildPhaseHandler : GamePhaseHandler<BuildPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private Canvas VisitButtons;

    [Header("Spectating")]
    [SerializeField] private List<SpectatingPlayerDisplay> spectationSpots = new List<SpectatingPlayerDisplay>();
    [SerializeField] private Dictionary<SpectatingPlayerDisplay, PlayerID> spectationSpotsDict;
    private const string SpectatingHeader = "Spectating";
    private PlayerID currentlySpectating;               // who we are spectating!
    private bool isSpectating;
    private List<PlayerID> spectatingPlayers;  // who we are getting spectated by!

    public override void OnPhaseEntered()
    {
        SetTargetPlayer(Game.ClientID);
        Game.NetworkChannel.StartListening(SpectatingHeader, OnSpectatingMsgReceived);
        buildingMenu.CanBuildBuilding = (building) => !visiting && Phase.CanAffordBuilding(building);
        buildingMenu.OnPlaceBuilding += Phase.PlaceBuilding;
        VisitButtons.gameObject.SetActive(true);
    }

    private bool visiting = false;
    public void UI_SetTargetPlayer(int id) => SetTargetPlayer((PlayerID)id);
    public void SetTargetPlayer(PlayerID player)
    {
        visiting = player != Game.ClientID;
        buildingMenu.gameObject.SetActive(!visiting);
        viewer.TargetPlayer = player;
    }

    public override void OnPhaseExited()
    {
        SetTargetPlayer(PlayerID.None);
        Game.NetworkChannel.StopListening(SpectatingHeader);
        buildingMenu.CanBuildBuilding = null;
        buildingMenu.OnPlaceBuilding -= Phase.PlaceBuilding;
        VisitButtons.gameObject.SetActive(false);
    }


    #region Spectating
    private void OnSpectatingMsgReceived(NetworkMessage message)
    {
        if((bool)message.content) spectatingPlayers.Add(message.sender);
        else spectatingPlayers.Remove(message.sender);
        UpdateSpectatingPlayers();
    }

    private void UpdateSpectatingPlayers()
    {
        if(spectationSpotsDict.Count <= 0)  // populate dict if empty
            for (int i = 0; i < spectationSpots.Count; i++)
                spectationSpotsDict.Add(spectationSpots[i], PlayerID.None);

        for (int i = 0; i < spectatingPlayers.Count; i++)
        {
            PlayerID spectatorID = spectatingPlayers[i];
            if (spectationSpotsDict.ContainsValue(spectatorID)) continue;

            // pick an available spot to spawn
            foreach (var spectationDisplay in spectationSpotsDict.Keys)
            {
                if (spectationSpotsDict[spectationDisplay] == PlayerID.None)
                {
                    spectationSpotsDict[spectationDisplay] = spectatorID;
                    // spawn spectator

                    // setup spectator
                    spectationDisplay.Setup(Game.PlayerData[spectatorID].Faction);
                }
            }
        }
        int j = 0;
        foreach (SpectatingPlayerDisplay spectationDisplay in spectationSpotsDict.Keys)
        {
            j++;
            if (j > spectatingPlayers.Count)    // all players have a spot now
            {
                spectationSpotsDict[spectationDisplay] = PlayerID.None;
                spectationDisplay.RemoveSpectator();
                continue;
            }
            if (spectationSpotsDict.ContainsValue(spectatingPlayers[j])) continue;  // player is already displayed
            if (spectationSpotsDict[spectationDisplay] == PlayerID.None)    // spot is available
            {
                spectationSpotsDict[spectationDisplay] = spectatingPlayers[j];
                // setup spectator
                spectationDisplay.Setup(Game.PlayerData[spectatingPlayers[j]].Faction);
            }
        }
    }

    public void StartSpectating(PlayerID playerID)
    {
        if (isSpectating) StopSpectating(currentlySpectating);

        ToggleSpectating(playerID, true);
        isSpectating = true;
        currentlySpectating = playerID;
    }
    public void StopSpectating(PlayerID playerID)
    {
        ToggleSpectating(playerID, false);
        isSpectating = false;
    }

    private void ToggleSpectating(PlayerID playerID, bool spectating) => Game.NetworkChannel.SendMessage(SpectatingHeader, spectating, playerID);
    #endregion
}
