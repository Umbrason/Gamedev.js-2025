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
    private PlayerID currentTargetPlayer = PlayerID.None;               // who we are spectating! Also reflected in viewer.TargetPlayer
    private bool visiting = false;

    public override void OnPhaseEntered()
    {
        Game.NetworkChannel.StartListening(VisitingHeader, OnOtherPlayerVisitingStatusChanged);
        SetTargetPlayer(Game.ClientID, false);
        buildingMenu.CanBuildBuilding = (building) => !visiting && Phase.CanAffordBuilding(building);
        buildingMenu.OnPlaceBuilding += Phase.PlaceBuilding;
        buildingMenu.gameObject.SetActive(true);
        VisitButtons.Refresh();
        VisitButtons.gameObject.SetActive(true);
        VisitButtons.OnClick += (playerID) => SetTargetPlayer(playerID);
        missionsDisplay.OnClickMission += (missionID) => ShowPledgeScreen(new() { missionID });
        missionsDisplay.Show();
        Phase.OnHarvestResource += OnHarvest;
        Phase.OnConsumeResource += OnConsume;
        Phase.OnPledgeResource += OnPledgeResource;

        switch (Game.ClientPlayerData.Role)
        {
            case PlayerRole.Balanced: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._19_RoundStart_GoodVersion, Game.ClientID); break;
            case PlayerRole.Selfish: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._47_RoundStart_SusVersion, Game.ClientID); break;
        }
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
        buildingMenu.gameObject.SetActive(false);
        VisitButtons.OnClick += (playerID) => SetTargetPlayer(playerID);
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
    public void SetTargetPlayer(PlayerID newTargetPlayer, bool playSFX = true)
    {
        if (currentTargetPlayer == newTargetPlayer) return;
        visiting = newTargetPlayer != Game.ClientID && newTargetPlayer != PlayerID.None;
        var isClient = newTargetPlayer == Game.ClientID;
        var isAnyPlayer = newTargetPlayer != PlayerID.None;
        var isOtherPlayer = isAnyPlayer && !isClient;

        playerDisplayProvider.IslandOwner = Game.PlayerData.GetValueOrDefault(newTargetPlayer)?.Faction ?? PlayerFaction.None;

        if (playSFX)
        {
            switch (playerDisplayProvider.IslandOwner)
            {
                case PlayerFaction.Bumbi: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._27_LookAtBumbiIsland, Game.ClientID); break;
                case PlayerFaction.Gumbi: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._30_LookAtGumbiIsland, Game.ClientID); break;
                case PlayerFaction.Lyki: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._28_LookAtLykiIsland, Game.ClientID); break;
                case PlayerFaction.Pigyn: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._29_LookAtPigynIsland, Game.ClientID); break;
                case PlayerFaction.PomPom: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._32_LookAtPomPomIsland, Game.ClientID); break;
                case PlayerFaction.Seltas: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._31_LookAtSeltasIsland, Game.ClientID); break;
            }
        }

        #region send notification to other player when visiting them
        if (isOtherPlayer)
        {
            Game.NetworkChannel.SendMessage(VisitingHeader, false, currentTargetPlayer);
            if (isOtherPlayer)
                Game.NetworkChannel.SendMessage(VisitingHeader, true, newTargetPlayer);
        }
        currentTargetPlayer = newTargetPlayer;
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
        viewer.TargetPlayer = newTargetPlayer;
    }

    private List<PlayerFaction> otherVisitingPlayers = new();
    private void OnOtherPlayerVisitingStatusChanged(NetworkMessage message)
    {
        var isPresent = (bool)message.content;
        var faction = Game.PlayerData[message.sender].Faction;
        if (isPresent)
        {
            otherVisitingPlayers.Add(faction);
            if (currentTargetPlayer == Game.ClientID) playerDisplayProvider.Show(faction, true);

            switch (faction)
            {
                case PlayerFaction.Bumbi: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._05_BumbiJoinsLobby, Game.ClientID); break;
                case PlayerFaction.Gumbi: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._11_GumbiJoinsLobby, Game.ClientID); break;
                case PlayerFaction.Lyki: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._07_LykiJoinsLobby, Game.ClientID); break;
                case PlayerFaction.Pigyn: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._09_PigynJoinsLobby, Game.ClientID); break;
                case PlayerFaction.PomPom: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._15_PomPomJoinsLobby, Game.ClientID); break;
                case PlayerFaction.Seltas: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._13_SeltasJoinsLobby, Game.ClientID); break;
            }
        }
        else
        {
            otherVisitingPlayers.Remove(faction);
            if (currentTargetPlayer == Game.ClientID) playerDisplayProvider.Hide(faction);

            switch (faction)
            {
                case PlayerFaction.Bumbi: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._06_BumbiLeavesLobby, Game.ClientID); break;
                case PlayerFaction.Gumbi: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._12_GumbiLeavesLobby, Game.ClientID); break;
                case PlayerFaction.Lyki: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._08_LykiLeavesLobby, Game.ClientID); break;
                case PlayerFaction.Pigyn: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._10_PigynLeavesLobby, Game.ClientID); break;
                case PlayerFaction.PomPom: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._16_PomPomLeavesLobby, Game.ClientID); break;
                case PlayerFaction.Seltas: SoundAndMusicController.Instance.PlaySFX(SoundAndMusicController.Instance.SfxClips._14_SeltasLeavesLobby, Game.ClientID); break;
            }
        }
    }
    #endregion
}
