using System;
using UnityEngine;

public class AccusationPhaseHandler : GamePhaseHandler<AccusationPhase>
{
    [SerializeField] private AccusationPickerDialogue accusationPickerDialogue;

    [SerializeField] private AccusationVoteDialogue accusationVoteDialogue;

    private bool alredyMadeAnAccusation;

    public override void OnPhaseEntered()
    {
        alredyMadeAnAccusation = false;
        accusationPickerDialogue.gameObject.SetActive(true);
        accusationPickerDialogue.OnAccusationMade += OnAccusationMade;
        accusationVoteDialogue.OnVote += OnVoteForAccusation;
        Phase.OnAccusationVoteStarted += OnAccusationVoteStarted;

        SoundAndMusicController.Instance.PlaySFX(SFXType._04_PopUpOpens, Game.ClientID);
    }

    public override void OnPhaseExited()
    {
        accusationPickerDialogue.gameObject.SetActive(false);
        accusationVoteDialogue.gameObject.SetActive(false);
        accusationPickerDialogue.OnAccusationMade -= OnAccusationMade;
        accusationVoteDialogue.OnVote -= OnVoteForAccusation;
        Phase.OnAccusationVoteStarted -= OnAccusationVoteStarted;

        SoundAndMusicController.Instance.PlaySFX(SFXType._43_PopUpCloses, Game.ClientID);
    }

    private void OnAccusationVoteStarted(PlayerID[] obj)
    {
        voted = false;
        accusationVoteDialogue.Show(obj);
    }

    bool voted = false;
    public void OnVoteForAccusation(bool vote)
    {
        if (voted) return;
        voted = true;
        Phase.Vote(vote);
    }

    void OnAccusationMade(PlayerID[] Accusation)
    {
        if (!alredyMadeAnAccusation)
        {
            accusationPickerDialogue.gameObject.SetActive(false);
            Phase.Accuse(Accusation);
            alredyMadeAnAccusation = true;
            accusationPickerDialogue.DisableAccusationOpprtunity();
        }
    }
}
