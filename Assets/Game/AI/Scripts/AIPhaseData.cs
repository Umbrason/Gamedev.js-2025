using System.Collections;
using UnityEngine;

public abstract class AIPhaseData<T> : ScriptableObject where T : IGamePhase
{
    public abstract IEnumerator PlayingPhase(T Phase, AIPlayer AI);
}
