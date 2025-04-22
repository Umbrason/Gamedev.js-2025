using System;
using TMPro;
using UnityEngine;

public class VotePhaseHandler : GamePhaseHandler<VotePhase>
{
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private BuildingView buildingPreview;
    [SerializeField] private PlayerDisplay playerDisplayProvider;

    private bool phaseActive = false;
    void Start() => canvas.gameObject.SetActive(false);

    #region Boring
    public override void OnPhaseEntered()
    {
        Phase.OnPetitionChanged += PetitionChanged;
        Phase.OnPetitionDecided += PetitionDecided;
        canvas.gameObject.SetActive(true);
        phaseActive = true;
        for (int i = 1; i < Enum.GetNames(typeof(PlayerFactions)).Length; i++)
        {
            // method checks for duplicates etc.
            if(Game.ClientPlayerData.Faction == (PlayerFactions)i)
            {
                continue;
            }
            playerDisplayProvider.Show((PlayerFactions)i);
        }
    }
    public override void OnPhaseExited()
    {
        Phase.OnPetitionChanged -= PetitionChanged;
        Phase.OnPetitionDecided -= PetitionDecided;
        canvas.gameObject.SetActive(false);
        phaseActive = false;
        for (int i = 1; i < Enum.GetNames(typeof(PlayerFactions)).Length; i++)
        {
            // Should we avoid PlayerID here? don't think so..
            // Should be handled already but idk
            playerDisplayProvider.Hide((PlayerFactions)i);
        }
    }
    #endregion

    private void PetitionChanged()
    {
        var hasPetition = Phase.CurrentPetition != null;
        canvas.gameObject.SetActive(hasPetition);
        buildingPreview.gameObject.SetActive(hasPetition);
        if (!hasPetition) return;

        buildingNameText.text = Phase.CurrentPetition.Building.ToString();
        buildingPreview.Data = Phase.CurrentPetition.Building;
        buildingPreview.transform.position = HexOrientation.Active * Phase.CurrentPetition.Position;
        // show other stuff here too, not playername tho
    }
    private void PetitionDecided(bool success)
    {
        //Animation here
    }

    public void SubmitVote(int vote)
    {
        if (!phaseActive) return;
        Phase.SubmitVote(vote);
    }
}
