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

    public IEnumerator Loop()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (Game.BalancedFactionGoals.All(goal => goal.Complete) || Game.SelfishFactionGoals.All(goal => goal.Complete))
            Game.TransitionPhase(new GameOverPhase());
        else Game.TransitionPhase(new PetitionPhase());
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }
}
