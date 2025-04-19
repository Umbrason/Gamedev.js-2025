using System;
using TMPro;
using UnityEngine;

public class VotePhaseHandler : GamePhaseHandler<VotePhase>
{
    [SerializeField] private TextMeshProUGUI buildingNameText;


    // private int selectedVote = 0;
    private bool phaseActive = false;

    #region Boring
    public override void OnPhaseEntered()
    {
        base.OnPhaseEntered();  // guess I'll leave these here just in case
        phaseActive = true;
    }
    public override void OnPhaseExited()
    {
        base.OnPhaseExited();
        phaseActive = false;
    }

    private void OnEnable()
    {
        Phase.OnPetitionChanged += PetitionChanged;
        Phase.OnPetitionDecided += PetitionDecided;
    }
    private void OnDisable()
    {
        Phase.OnPetitionChanged -= PetitionChanged;
        Phase.OnPetitionDecided -= PetitionDecided;
    }
    #endregion

    private void PetitionChanged()
    {
        buildingNameText.text = Phase.CurrentPetition.Building.ToString();
        // show other stuff here too, not playername tho
    }
    private void PetitionDecided(bool success)
    {
        throw new NotImplementedException();
    }

    public void SubmitVote(int vote)
    {
        if (!phaseActive) return;
        Phase.SubmitVote(vote);
    }
}
