using UnityEngine;

public class BuildPhaseHandler : GamePhaseHandler<BuildPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private PlayerIDButtons VisitButtons;

    [Header("Visiting")]
    [SerializeField] private PlayerDisplayProvider playerDisplayProvider;
    private const string VisitingHeader = "Visiting";
    private PlayerID targetPlayer = PlayerID.None;               // who we are spectating! Also reflected in viewer.TargetPlayer
    private bool visiting = false;

    public override void OnPhaseEntered()
    {
        Game.NetworkChannel.StartListening(VisitingHeader, OnVisitingMsgReceived);
        SetTargetPlayer(Game.ClientID);
        buildingMenu.CanBuildBuilding = (building) => !visiting && Phase.CanAffordBuilding(building);
        buildingMenu.OnPlaceBuilding += Phase.PlaceBuilding;
        buildingMenu.gameObject.SetActive(true);
        VisitButtons.Refresh();
        VisitButtons.gameObject.SetActive(true);
        VisitButtons.OnClick += SetTargetPlayer;
    }

    public override void OnPhaseExited()
    {
        SetTargetPlayer(PlayerID.None);
        Game.NetworkChannel.StopListening(VisitingHeader);
        buildingMenu.CanBuildBuilding = null;
        buildingMenu.OnPlaceBuilding -= Phase.PlaceBuilding;
        VisitButtons.OnClick -= SetTargetPlayer;
    }


    #region Visiting
    public void SetTargetPlayer(PlayerID playerID)
    {
        if (targetPlayer == playerID) return;
        if (targetPlayer != PlayerID.None && targetPlayer != Game.ClientID)
        {
            Game.NetworkChannel.SendMessage(VisitingHeader, false, targetPlayer);
            if (playerID != PlayerID.None && playerID != Game.ClientID)
                Game.NetworkChannel.SendMessage(VisitingHeader, true, targetPlayer);
        }
        visiting = playerID != Game.ClientID && playerID != PlayerID.None;
        buildingMenu.gameObject.SetActive(!visiting);
        viewer.TargetPlayer = playerID;
        targetPlayer = playerID;
    }
    private void OnVisitingMsgReceived(NetworkMessage message)
    {
        playerDisplayProvider.SwitchDisplayOwner(Game.PlayerData[message.sender].Faction, (bool)message.content);
    }
    #endregion
}
