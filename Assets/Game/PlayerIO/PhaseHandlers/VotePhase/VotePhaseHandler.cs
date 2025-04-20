using System;
using TMPro;
using UnityEngine;

public class VotePhaseHandler : GamePhaseHandler<VotePhase>
{
    [SerializeField] private TextMeshProUGUI buildingNameText;
    [SerializeField] private Canvas canvas;
    [SerializeField] private BuildingView buildingPreview;

    private bool phaseActive = false;
    void Start() => canvas.gameObject.SetActive(false);

    #region Boring
    public override void OnPhaseEntered()
    {
        Phase.OnPetitionChanged += PetitionChanged;
        Phase.OnPetitionDecided += PetitionDecided;
        canvas.gameObject.SetActive(true);
        phaseActive = true;
    }
    public override void OnPhaseExited()
    {
        Phase.OnPetitionChanged -= PetitionChanged;
        Phase.OnPetitionDecided -= PetitionDecided;
        canvas.gameObject.SetActive(false);
        phaseActive = false;
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
