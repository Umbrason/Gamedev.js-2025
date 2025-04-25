using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotePhaseHandler : GamePhaseHandler<VotePhase>
{
    [SerializeField] private TMP_Text buildingNameText;
    [SerializeField] private Image buildingIcon;
    [SerializeField] private BuildingSpriteLib BuildingIcons;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GhostBuildingView buildingPreview;
    [SerializeField] private PlayerDisplay playerDisplayProvider;
    [SerializeField] private PlayerIslandViewer islandViewer;
    [SerializeField] private ResourceSourceDisplay resourceSourceDisplay;
    [SerializeField] private GameObject Buttons;

    [Header("VoteDisplay")]
    [SerializeField] private Transform voteDisplayParent;
    private List<PlayerVoteDisplay> instances = new();
    [SerializeField] private PlayerVoteDisplay voteDisplayPrefab;
    [SerializeField] private GameObject votedAccept, votedRefuse;

    private bool phaseActive = false;
    void Start() => canvas.gameObject.SetActive(false);

    #region Boring
    public override void OnPhaseEntered()
    {
        Phase.OnPetitionChanged += PetitionChanged;
        Phase.OnPetitionDecided += PetitionDecided;
        Phase.OnVoted += OnVoted;
        canvas.gameObject.SetActive(true);
        phaseActive = true;
        voteDisplayParent.gameObject.SetActive(false);
    }
    public override void OnPhaseExited()
    {
        SetDisplay(PlayerID.None);
        Phase.OnPetitionChanged -= PetitionChanged;
        Phase.OnPetitionDecided -= PetitionDecided;
        Phase.OnVoted -= OnVoted;
        canvas.gameObject.SetActive(false);
        phaseActive = false;
        Buttons.gameObject.SetActive(true);
        voteDisplayParent.gameObject.SetActive(false);
    }
    #endregion

    private void PetitionChanged()
    {
        foreach (var instance in instances)
            Destroy(instance.gameObject);
        instances.Clear();
        votedAccept.SetActive(false);
        votedRefuse.SetActive(false);
        Buttons.gameObject.SetActive(true);
        voteDisplayParent.gameObject.SetActive(false);
        var hasPetition = Phase.CurrentPetition != null;
        canvas.gameObject.SetActive(hasPetition);
        buildingPreview.gameObject.SetActive(hasPetition);
        SetDisplay(Phase.CurrentPetition?.PlayerID ?? PlayerID.None);
        if (!hasPetition) return;
        buildingNameText.text = Phase.CurrentPetition.Building.ToString();
        buildingIcon.sprite = BuildingIcons[Phase.CurrentPetition.Building];
        buildingPreview.Data = Phase.CurrentPetition.Building;
        var resourceSources = Phase.CurrentPetition.ResourceSources;
        // TODO: replace this with icons
        resourceSourceDisplay.Sources = resourceSources;
        buildingPreview.transform.position = Phase.CurrentPetition.Position.WorldPositionCenter;
    }

    private void SetDisplay(PlayerID playerID)
    {
        islandViewer.TargetPlayer = playerID;
        for (int i = 1; i < Enum.GetNames(typeof(PlayerFaction)).Length; i++)
            playerDisplayProvider.Hide((PlayerFaction)i);
        playerDisplayProvider.IslandOwner = Game.ClientPlayerData.Faction;
        if (playerID == PlayerID.None) return;
        playerDisplayProvider.IslandOwner = Game.PlayerData[playerID].Faction;
        for (int i = 1; i < Enum.GetNames(typeof(PlayerFaction)).Length; i++)
        {
            if (Game.PlayerData[playerID].Faction == (PlayerFaction)i) continue;
            playerDisplayProvider.Show((PlayerFaction)i);
        }
    }

    private void PetitionDecided(bool success)
    {
        // TODO: Animation here @Cathy

    }

    private void OnVoted(PlayerID id, int vote)
    {
        // display vote player with icon
        PlayerVoteDisplay votedDisplay = Instantiate(voteDisplayPrefab, voteDisplayParent);
        instances.Add(votedDisplay);
        votedDisplay.Setup(Game.PlayerData[id].Faction, vote);
    }

    // TODO: @Cathy: do things with buttons maybe idk
    public void SubmitVote(int vote)
    {
        if (!phaseActive) return;
        Phase.SubmitVote(vote);
        votedAccept.SetActive(vote > 0);
        votedRefuse.SetActive(vote < 0);
        Buttons.gameObject.SetActive(false);
        voteDisplayParent.gameObject.SetActive(true);
    }
}
