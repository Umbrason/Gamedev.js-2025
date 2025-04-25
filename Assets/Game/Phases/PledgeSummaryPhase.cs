using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PledgeSummaryPhase : IGamePhase, ITimedPhase
{
    public GameInstance Game { private get; set; }

    public Dictionary<PlayerID, Dictionary<Resource, int>> OfferedResources;

    public float TimeRemaining => startTime - Time.unscaledTime + Duration;
    public float Duration => 10f;
    public float startTime { get; set; } = -1000f;

    public IEnumerator OnEnter()
    {
        yield return null;
    }

    const bool BalancedTeamHasPriority = false;
    public IEnumerator Loop()
    {
        startTime = Time.unscaledTime;
        yield return new WaitForSecondsRealtime(Duration);
        if (Game.BalancedFactionGoals.All(goal => goal.Complete) || Game.SelfishFactionGoals.All(goal => goal.Complete))
            Game.TransitionPhase(new GameOverPhase(BalancedTeamHasPriority ?
                                    (Game.BalancedFactionGoals.All(goal => goal.Complete) ? PlayerRole.Balanced : PlayerRole.Selfish) :
                                    (Game.SelfishFactionGoals.All(goal => goal.Complete) ? PlayerRole.Selfish : PlayerRole.Balanced)));
        else Game.TransitionPhase(new AccusationPhase());
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }

    public void Skip() { }

    public bool CanSkip() => false;
}
