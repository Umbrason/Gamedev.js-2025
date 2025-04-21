using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PledgeSummaryPhase : IGamePhase, ITimedPhase
{
    public GameInstance Game { private get; set; }

    public Dictionary<PlayerID, ResourcePledge> Pledges;

    public float TimeRemaining => _startTime - Time.unscaledTime + Duration;
    public float Duration => 10f;
    private float _startTime = -1000f;

    public IEnumerator OnEnter()
    {
        yield return null;
    }

    const bool BalancedTeamHasPriority = false;
    public IEnumerator Loop()
    {
        _startTime = Time.unscaledTime;
        yield return new WaitForSecondsRealtime(Duration);
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

    public PledgeSummaryPhase(Dictionary<PlayerID, ResourcePledge> pledges)
    {
        Pledges = pledges;
    }
}
