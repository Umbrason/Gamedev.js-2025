using System.Collections;
using System.Linq;

public class GameOverPhase : IGamePhase
{
    const bool BalancedTeamHasPriority = false;
    public GameInstance Game { private get; set; }
    public PlayerRole WinnerRole => BalancedTeamHasPriority ?
                                    (Game.BalancedFactionGoals.All(goal => goal.Complete) ? PlayerRole.Balanced : PlayerRole.Selfish) :
                                    (Game.SelfishFactionGoals.All(goal => goal.Complete) ? PlayerRole.Selfish : PlayerRole.Balanced);
    public IEnumerator OnEnter()
    {
        yield return null;
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }
}
