using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotePhaseHandler : GamePhaseHandler<VotePhase>
{
    [SerializeField] private TMP_Text buildingNameText;
    [SerializeField] private TMP_Text resourceSourcesText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private BuildingView buildingPreview;
    [SerializeField] private PlayerDisplay playerDisplayProvider;
    [SerializeField] private PlayerIslandViewer islandViewer;
    [SerializeField] private List<Button> voteBtns;

    [Header("VoteDisplay")]
    [SerializeField] private Transform voteDisplayParent;
    [SerializeField] private PlayerVoteDisplay voteDisplayPrefab;
    [SerializeField] private GameObject votedAccept, votedRefuse;

    private bool phaseActive = false;
    void Start() => canvas.gameObject.SetActive(false);

    #region Boring
    public override void OnPhaseEntered()
    {
        Phase.OnPetitionChanged += PetitionChanged;
        Phase.OnPetitionDecided += PetitionDecided;
        Phase.OnVoted += Voted;
        canvas.gameObject.SetActive(true);
        phaseActive = true;
    }
    public override void OnPhaseExited()
    {
        SetDisplay(PlayerID.None);
        Phase.OnPetitionChanged -= PetitionChanged;
        Phase.OnPetitionDecided -= PetitionDecided;
        Phase.OnVoted -= Voted;
        canvas.gameObject.SetActive(false);
        phaseActive = false;
    }
    #endregion

    private void PetitionChanged()
    {
        voteDisplayParent.DeleteChildren();
        for (int i = 0; i < voteBtns.Count; i++) voteBtns[i].interactable = true;

        var hasPetition = Phase.CurrentPetition != null;
        canvas.gameObject.SetActive(hasPetition);
        buildingPreview.gameObject.SetActive(hasPetition);
        SetDisplay(Phase.CurrentPetition?.PlayerID ?? PlayerID.None);
        if (!hasPetition) return;
        buildingNameText.text = Phase.CurrentPetition.Building.ToString();
        buildingPreview.Data = Phase.CurrentPetition.Building;
        var resourceSources = Phase.CurrentPetition.ResourceSources;
        // TODO: replace this with icons
        var resourceSourceTextLines = new List<string>();
        foreach (var (playerID, resources) in resourceSources)
        {
            var resStrings = resources.Select((pair) => $"{pair.Value} {pair.Key}").ToList();
            var text = $"{Game.PlayerData[playerID].Faction}: {string.Join(", ", resStrings)}";
            resourceSourceTextLines.Add(text);
        }
        resourceSourcesText.text = string.Join("\n", resourceSourceTextLines);
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

    private void Voted(PlayerID id, int vote)
    {
        // display vote player with icon
        PlayerVoteDisplay go = Instantiate(voteDisplayPrefab, voteDisplayParent);
        go.Setup(Game.PlayerData[id].Faction, vote);
    }

    // TODO: @Cathy: do things with buttons maybe idk
    public void SubmitVote(int vote)
    {
        if (!phaseActive) return;
        Phase.SubmitVote(vote);
        for (int i = 0; i < voteBtns.Count; i++) voteBtns[i].interactable = false;
        votedAccept.SetActive(vote > 0);
        votedRefuse.SetActive(vote < 0);
    }
}
