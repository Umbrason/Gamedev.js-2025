using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "AIConfig", menuName = "Scriptable Objects/AI/Config")]
public class AIConfigData : ScriptableObject
{
    [Min(0f), SerializeField] private float minActionDelay = 0f;
    [Min(0f), SerializeField] private float maxActionDelay = 0f;


    [SerializeField] private AIBuildPhaseData AIBuildPhase; 
    [SerializeField] private AIPetitionPhaseData AIPetitionPhase; 
    [SerializeField] private AIVotePhaseData AIVotePhase; 
    [SerializeField] private AIAccusationPhaseData AIAccusationPhase;


    public float ActionDelay => Random.Range(minActionDelay, maxActionDelay);

    public IEnumerator PlayingPhase(IGamePhase Phase, GameInstance GameInstance)
    {
        //dirty but idk how to do better
        if(Phase is BuildPhase buildPhase) return AIBuildPhase?.PlayingPhase(buildPhase, this, GameInstance);
        else if(Phase is PetitionPhase petitionPhase) return AIPetitionPhase?.PlayingPhase(petitionPhase, this, GameInstance);
        else if(Phase is VotePhase votePhase) return AIVotePhase?.PlayingPhase(votePhase, this, GameInstance);
        else if(Phase is AccusationPhase accusationPhase) return AIAccusationPhase?.PlayingPhase(accusationPhase, this, GameInstance);

        return null;
    }
}
