using System.Collections.Generic;
using UnityEngine;

public class BuildPhaseHandler : GamePhaseHandler<BuildPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private PlayerIDButtons VisitButtons;
    [SerializeField] private GoalResourcePledgeScreen pledgeScreen;
    [SerializeField] private MissionsDisplay missionsDisplay;
    [SerializeField] private ResourceChangeAnimation resourceChangeAnimation;

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
        Phase.OnHarvestResource += OnHarvest;
        Phase.OnConsumeResource += OnConsume;
        Phase.OnPledgeResource += OnPledgeResource;
    }

    private void OnPledgeResource(SharedGoalID goalID, Resource resource, int amount)
    {
        for (int i = 0; i < Mathf.Abs(amount); i++)
        {
            if (amount > 0) resourceChangeAnimation.Spawn(resource, Game.ClientID, goalID);
            else resourceChangeAnimation.Spawn(resource, goalID, Game.ClientID);
        }
    }

    private void OnConsume(HexPosition position, Building building, Resource resource, int amount)
    {
        for (int i = 0; i < amount; i++)
            resourceChangeAnimation.Spawn(resource, Game.ClientID, position);
    }

    private void OnHarvest(HexPosition position, Building building, Resource resource, int amount)
    {
        for (int i = 0; i < amount; i++)
            resourceChangeAnimation.Spawn(resource, position, Game.ClientID);
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
        Phase.OnHarvestResource -= OnHarvest;
        Phase.OnConsumeResource -= OnConsume;
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
        var isClient = playerID == Game.ClientID;
        var isAnyPlayer = playerID != PlayerID.None;
        var isOtherPlayer = isAnyPlayer && playerID != Game.ClientID;

        playerDisplayProvider.IslandOwner = Game.PlayerData.GetValueOrDefault(playerID)?.Faction ?? PlayerFaction.None;

        #region send notification to other player when visiting them
        if (isOtherPlayer)
        {
            Game.NetworkChannel.SendMessage(VisitingHeader, false, targetPlayer);
            if (playerID != PlayerID.None && playerID != Game.ClientID)
                Game.NetworkChannel.SendMessage(VisitingHeader, true, targetPlayer);
        }
        targetPlayer = playerID;
        #endregion
        #region reset visitors
        foreach (var visitor in otherVisitingPlayers)
            playerDisplayProvider.Hide(visitor);
        if (isClient) foreach (var visitor in otherVisitingPlayers)
                playerDisplayProvider.Show(visitor, true);
        #endregion


        #region Show self as visitor when visiting other players
        if (isClient)
        {
            playerDisplayProvider.Hide(Game.ClientPlayerData.Faction);
        }
        if (isOtherPlayer)
        {
            playerDisplayProvider.Show(Game.ClientPlayerData.Faction);
        }
        #endregion

        buildingMenu.gameObject.SetActive(!visiting);
        viewer.TargetPlayer = playerID;
    }

    private List<PlayerFaction> otherVisitingPlayers = new();
    private void OnOtherPlayerVisitingStatusChanged(NetworkMessage message)
    {
        var isPresent = (bool)message.content;
        var faction = Game.PlayerData[message.sender].Faction;
        if (isPresent)
        {
            otherVisitingPlayers.Add(faction);
            if (targetPlayer == Game.ClientID) playerDisplayProvider.Show(faction, true);
        }
        else
        {
            otherVisitingPlayers.Remove(faction);
            if (targetPlayer == Game.ClientID) playerDisplayProvider.Hide(faction);
        }
    }
    #endregion
}
