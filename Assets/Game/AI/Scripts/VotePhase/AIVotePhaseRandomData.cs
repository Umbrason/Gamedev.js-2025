using System.Collections;
using UnityEditorInternal;
using UnityEngine;

[CreateAssetMenu(fileName = "VotePhaseRandom", menuName = "Scriptable Objects/AI/Vote/Random")]
public class AIVotePhaseRandomData : AIVotePhaseData
{
    [SerializeField, Range(0f, 1f)] float probabilityToAccept = 0.5f; 

    public override IEnumerator PlayingPhase(VotePhase Phase, AIConfigData Config, GameInstance GameInstance)
    {
        BuildingPetition petition = null;

        while (true)
        {  
            while (Phase.CurrentPetition == petition)//Wait until next petition begins
            {
                yield return null;
            }
            petition = Phase.CurrentPetition;
            if (petition == null) break;
            yield return new WaitForSeconds(Config.ActionDelay);
            Phase.SubmitVote(Random.value < probabilityToAccept ? 1 : -1);
        }
    }
}
