using System.Collections;

public class AccusationPhase : IGamePhase
{
    public GameInstance Game { private get; set; }
    public INetworkChannel NetworkChannel { private get; set; }

    public IEnumerator OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator OnExit()
    {
        throw new System.NotImplementedException();
    }
}