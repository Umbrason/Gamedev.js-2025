using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "VotePhaseRandom", menuName = "Scriptable Objects/AI/Vote/Random")]
public class AIVotePhaseRandomData : AIVotePhaseData
{
    [SerializeField, Range(0f, 1f)] float balanceProbabilityToAccept = 0.5f; 
    [SerializeField] bool balanceAlwaysVoteForSelf = true; 
    [SerializeField, Range(0f, 1f)] float selfishProbabilityToAccept = 0.5f;
    [SerializeField] bool selfishAlwaysVoteForSelf = true;


    public override IEnumerator PlayingPhase(VotePhase Phase, AIPlayer AI)
    {
        BuildingPetition petition = null;

        bool selfish = AI.GameInstance.ClientPlayerData.Role == PlayerRole.Selfish;

        float probabilityToAccept = selfish ? selfishProbabilityToAccept : balanceProbabilityToAccept;
        bool alwaysVoteForSelf = selfish ? selfishAlwaysVoteForSelf : balanceAlwaysVoteForSelf;

        while (true)
        {  
            while (Phase.CurrentPetition == petition)//Wait until next petition begins
            {
                yield return null;
            }
            petition = Phase.CurrentPetition;
            if (petition == null) break;
            yield return new WaitForSeconds(AI.Config.ActionDelay);
            
            bool vote = Random.value < probabilityToAccept;

            if(alwaysVoteForSelf && petition.PlayerID == AI.GameInstance.ClientID)
                vote = true;

            AI.Log(vote ? " agrees to petition" : "disagrees to petition");
            Phase.SubmitVote(vote ? 1 : -1);
        }
    }
}
