using UnityEngine;

public class BuildPhaseHandler : GamePhaseHandler<BuildPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private Canvas VisitButtons;

    [Header("Visiting")]
    [SerializeField] private PlayerDisplayProvider playerDisplayProvider;
    private const string VisitingHeader = "Visiting";
    private PlayerID currentlyVisiting;               // who we are spectating! Also reflected in viewer.TargetPlayer
    private bool visiting = false;

    public override void OnPhaseEntered()
    {
        StartVisiting(Game.ClientID);
        Game.NetworkChannel.StartListening(VisitingHeader, OnVisitingMsgReceived);
        buildingMenu.CanBuildBuilding = (building) => !visiting && Phase.CanAffordBuilding(building);
        buildingMenu.OnPlaceBuilding += Phase.PlaceBuilding;
        VisitButtons.gameObject.SetActive(true);
    }

    public override void OnPhaseExited()
    {
        StartVisiting(PlayerID.None);
        Game.NetworkChannel.StopListening(VisitingHeader);
        buildingMenu.CanBuildBuilding = null;
        buildingMenu.OnPlaceBuilding -= Phase.PlaceBuilding;
        VisitButtons.gameObject.SetActive(false);
    }


    #region Visiting
    public void UI_SetTargetPlayer(int id) => StartVisiting((PlayerID)id);
    public void StartVisiting(PlayerID playerID)
    {
        if (currentlyVisiting == playerID) return;
        if (visiting) ToggleVisiting(false);

        visiting = playerID != Game.ClientID;
        buildingMenu.gameObject.SetActive(!visiting);
        viewer.TargetPlayer = playerID;

        if (!visiting) return;
        currentlyVisiting = playerID;
        ToggleVisiting(true);

        void ToggleVisiting(bool spectating) =>
            Game.NetworkChannel.SendMessage(VisitingHeader, spectating, currentlyVisiting);
    }
    private void OnVisitingMsgReceived(NetworkMessage message)
    {
        playerDisplayProvider.SwitchDisplayOwner(Game.PlayerData[message.sender].Faction, (bool)message.content);
    }
    #endregion
}
