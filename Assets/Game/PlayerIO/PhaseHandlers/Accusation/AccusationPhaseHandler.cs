using System.Collections;
using System.Linq;
using UnityEngine;

public class AccusationPhaseHandler : GamePhaseHandler<AccusationPhase>
{
    [SerializeField] private GameObject accusationPanel;
    [SerializeField] private GameObject choicePanel;    // for those that can vote


    public override void OnPhaseEntered()
    {
        accusationPanel.SetActive(true);
        choicePanel.SetActive(Phase.isAccused);
        
    }

    public override void OnPhaseExited()
    {
        accusationPanel.SetActive(false);
        choicePanel.SetActive(Phase.isAccused); 
    }

    public void Vote(bool agree)
    {
        // Handle the player's vote
        Debug.Log($"{Game.ClientPlayerData.Nickname} voted: {(agree ? "Agree" : "Disagree")}");
        Phase.Vote(agree);
    }
}
