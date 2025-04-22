using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildPhaseHandler : GamePhaseHandler<BuildPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private PlayerIDButtons VisitButtons;
    [SerializeField] private GoalResourcePledgeScreen pledgeScreen;
    [SerializeField] private MissionsDisplay missionsDisplay;

    [Header("Visiting")]
    [SerializeField] private PlayerDisplay playerDisplayProvider;
    private const string VisitingHeader = "Visiting";
    private PlayerID targetPlayer = PlayerID.None;               // who we are spectating! Also reflected in viewer.TargetPlayer
    private bool visiting = false;

    public override void OnPhaseEntered()
    {
        Game.NetworkChannel.StartListening(VisitingHeader, OnOtherPlayerVisitingStatusChanged);
        SetTargetPlayer(Game.ClientID);
        buildingMenu.CanBuildBuilding = (building) => !visiting && Phase.CanAffordBuilding(building);
        buildingMenu.OnPlaceBuilding += Phase.PlaceBuilding;
        buildingMenu.gameObject.SetActive(true);
        VisitButtons.Refresh();
        VisitButtons.gameObject.SetActive(true);
        VisitButtons.OnClick += SetTargetPlayer;
        missionsDisplay.OnClickMission += (missionID) => ShowPledgeScreen(new() { missionID });
        missionsDisplay.Show();
    }

    public override void OnPhaseExited()
    {
        SetTargetPlayer(PlayerID.None);
        Game.NetworkChannel.StopListening(VisitingHeader);
        buildingMenu.CanBuildBuilding = null;
        buildingMenu.OnPlaceBuilding -= Phase.PlaceBuilding;
        VisitButtons.OnClick -= SetTargetPlayer;
        missionsDisplay.Hide();
        pledgeScreen.Hide();
    }

    void Update()
    {
        if (Phase != null && Phase.TimeRemaining < 15f && !pledgeScreen.Showing)
            ShowPledgeScreenAllGoals(false);
    }

    public void ShowPledgeScreenAllGoals(bool canHide = true)
    {
        var goals = new List<SharedGoalID>();
        for (int i = 0; i < Game.BalancedFactionGoals.Count; i++)
        {
            SharedGoalID id = new(PlayerRole.Balanced, i);
            goals.Add(id);
        }
        if (Game.ClientPlayerData.Role == PlayerRole.Selfish)
            for (int i = 0; i < Game.SelfishFactionGoals.Count; i++)
            {
                SharedGoalID id = new(PlayerRole.Selfish, i);
                goals.Add(id);
            }
        ShowPledgeScreen(goals, canHide);
    }
    public void ShowPledgeScreen(List<SharedGoalID> goals, bool canHide = true)
    {
        var goalsDict = new Dictionary<SharedGoalID, SharedGoal>();
        foreach (var goalID in goals)
            goalsDict[goalID] = goalID.GetGoal(Game);
        pledgeScreen.Show(goalsDict, Phase.PledgedResources.GetValueOrDefault(Game.ClientID), Phase.PledgeResource, canHide);
    }


    #region Visiting
    public void SetTargetPlayer(PlayerID playerID)
    {
        if (targetPlayer == playerID) return;
        visiting = playerID != Game.ClientID && playerID != PlayerID.None;
        if (targetPlayer != PlayerID.None && targetPlayer != Game.ClientID)
        {
            Game.NetworkChannel.SendMessage(VisitingHeader, false, targetPlayer);
            if (playerID != PlayerID.None && playerID != Game.ClientID)
                Game.NetworkChannel.SendMessage(VisitingHeader, true, targetPlayer);
        }
        targetPlayer = playerID;

        if (targetPlayer != PlayerID.None) playerDisplayProvider.IslandOwner = Game.PlayerData[targetPlayer].Faction;
        else playerDisplayProvider.IslandOwner = PlayerFactions.None;

        playerDisplayProvider.Hide(Game.ClientPlayerData.Faction);
        if (visiting && targetPlayer != PlayerID.None) playerDisplayProvider.Show(Game.ClientPlayerData.Faction);


        buildingMenu.gameObject.SetActive(!visiting);
        viewer.TargetPlayer = playerID;
    }
    private void OnOtherPlayerVisitingStatusChanged(NetworkMessage message)
    {
        var isVisiting = (bool)message.content;
        var faction = Game.PlayerData[message.sender].Faction;
        if (isVisiting) playerDisplayProvider.Show(faction);
        else playerDisplayProvider.Hide(faction);
    }
    #endregion
}
