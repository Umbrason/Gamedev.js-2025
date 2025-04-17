using System.Collections;
using System.Linq;
using UnityEngine;

public class PledgeSummaryPhase : IGamePhase
{
    public GameInstance Game { private get; set; }

    public IEnumerator OnEnter()
    {
        yield return null;
    }

    const bool BalancedTeamHasPriority = false;
    public IEnumerator Loop()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (Game.BalancedFactionGoals.All(goal => goal.Complete) || Game.SelfishFactionGoals.All(goal => goal.Complete))
            Game.TransitionPhase(new GameOverPhase(BalancedTeamHasPriority ?
                                    (Game.BalancedFactionGoals.All(goal => goal.Complete) ? PlayerRole.Balanced : PlayerRole.Selfish) :
                                    (Game.SelfishFactionGoals.All(goal => goal.Complete) ? PlayerRole.Selfish : PlayerRole.Balanced)));
        else Game.TransitionPhase(new PetitionPhase());
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }
}
