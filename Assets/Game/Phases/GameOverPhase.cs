using System.Collections;
using System.Linq;


//TODO: Implement a restart/exit for this
public class GameOverPhase : IGamePhase
{
    public GameInstance Game { private get; set; }
    public PlayerRole WinnerRole;

    public GameOverPhase(PlayerRole winnerRole)
    {
        WinnerRole = winnerRole;
    }

    public IEnumerator OnEnter()
    {
        yield return null;
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }
}
