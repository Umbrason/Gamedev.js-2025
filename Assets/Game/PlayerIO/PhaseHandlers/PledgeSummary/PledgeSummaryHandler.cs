using UnityEngine;

public class PledgeSummaryHandler : GamePhaseHandler<PledgeSummaryPhase>
{
    [SerializeField] PledgeView[] pledgeViews;
    [SerializeField] GameObject parent;

    public override void OnPhaseEntered()
    {
        foreach((PlayerID player, ResourcePledge pledge) in Phase.Pledges)
        {
            if ((int)player >= pledgeViews.Length || pledgeViews[(int)player] == null)
            {
                Debug.LogError("Missing pledge view");
                continue;
            }

            pledgeViews[(int)player].Pledge = pledge;
        }

        parent.SetActive(true);
    }

    public override void OnPhaseExited()
    {
        parent.SetActive(false);
    }
}
