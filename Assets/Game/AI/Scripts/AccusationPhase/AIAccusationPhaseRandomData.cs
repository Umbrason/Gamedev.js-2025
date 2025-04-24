using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AccusationPhaseRandom", menuName = "Scriptable Objects/AI/Accusation/Random")]
public class AIAccusationPhaseRandomData : AIAccusationPhaseData
{
    [SerializeField, Range(0f,1f)] private float AccuseProbability = 0.01f;
    [SerializeField, Range(0f,1f)] private float AgreeProbability = 0.5f;

    bool _voteStarted = false;
    PlayerID[] _accusedPlayers = null;

    public override IEnumerator PlayingPhase(AccusationPhase Phase, AIPlayer AI)
    {
        _voteStarted = false;
        Phase.OnAccusationVoteStarted += OnAccusationVoteStarted;

        yield return new WaitForSeconds(AI.Config.ActionDelay);

        if (AccuseProbability > UnityEngine.Random.value)
        {
            List<PlayerID> ids = new();
            foreach(PlayerID id in Enum.GetValues(typeof(PlayerID)))
            {
                if(id == PlayerID.None) continue;
                if (id == AI.GameInstance.ClientID) continue;//don't self accuse 
                ids.Add(id);
            }

            PlayerID[] accusations = new PlayerID[2];
            accusations[0] = ids.GetRandom();
            ids.Remove(accusations[0]);
            accusations[1] = ids.GetRandom();

            Phase.Accuse(accusations);
        }
        else
        {
            Phase.Accuse(null);
        }

        while (true)
        {
            yield return new WaitUntil(() => _voteStarted);
            yield return new WaitForSeconds(AI.Config.ActionDelay);

            bool vote = UnityEngine.Random.value <= AgreeProbability;
            //AI.Log("votes " + (vote ? "yes" : "no"));
            Phase.Vote(vote);
        }

    }

    private void OnAccusationVoteStarted(PlayerID[] accusedPlayers)
    {
        _voteStarted = true;
        _accusedPlayers = accusedPlayers;
    }
}
