using System.Collections;
using UnityEngine;

public class RoundTransitionPhase : IGamePhase
{
    public GameInstance Game { private get; set; }

    public IEnumerator OnEnter()
    {
        yield return null;
    }

    public IEnumerator Loop()
    {
        yield return new WaitForSecondsRealtime(5f);
        Game.TransitionPhase(new PetitionPhase());
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }
}
