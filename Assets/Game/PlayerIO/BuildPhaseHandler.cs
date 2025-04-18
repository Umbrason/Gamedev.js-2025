using UnityEngine;

public class BuildPhaseHandler : GamePhaseHandler<BuildPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    public override void OnPhaseEntered()
    {
        SetTargetPlayer(Game.ClientID);
    }

    private bool visiting = false;
    public void UI_SetTargetPlayer(int id) => SetTargetPlayer((PlayerID)id);
    public void SetTargetPlayer(PlayerID player)
    {
        visiting = player == Game.ClientID;
        if (visiting) ; //disable UI
        viewer.TargetPlayer = player;
    }

    public override void OnPhaseExited()
    {
        SetTargetPlayer(PlayerID.None);
    }
}
