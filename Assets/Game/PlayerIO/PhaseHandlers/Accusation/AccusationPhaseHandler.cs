using UnityEngine;

public class AccusationPhaseHandler : GamePhaseHandler<AccusationPhase>
{
    [SerializeField] private AccusationPickerDialogue accusationPickerDialogue;

    public override void OnPhaseEntered()
    {
        accusationPickerDialogue.gameObject.SetActive(true);
        accusationPickerDialogue.OnAccusationMade += OnAccusationMade;
    }

    public override void OnPhaseExited()
    {
        accusationPickerDialogue.gameObject.SetActive(false);
        accusationPickerDialogue.OnAccusationMade -= OnAccusationMade;
    }

    void OnVoteForAccusation(PlayerID[] Accusation)
    {

    }

    void OnAccusationMade(PlayerID[] Accusation)
    {
        accusationPickerDialogue.gameObject.SetActive(false);
        Phase.Accuse(Accusation);
    }
}
