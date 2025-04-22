using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "AccusationPhaseRandom", menuName = "Scriptable Objects/AI/Accusation/Random")]
public class AIAccusationPhaseRandomData : AIAccusationPhaseData
{
    [SerializeField, Range(0f,1f)] private float AgreeProbability = 0.5f;
    public override IEnumerator PlayingPhase(AccusationPhase Phase, AIConfigData Config, GameInstance GameInstance)
    {
        yield return new WaitForSeconds(Config.ActionDelay);
        Phase.Vote(Random.value < AgreeProbability);
    }
}
