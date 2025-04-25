using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AccusationPhaseImproved", menuName = "Scriptable Objects/AI/Accusation/Improved")]
public class AIAccusationPhaseImprovedRandom : AIAccusationPhaseData
{
    [SerializeField, Range(0f, 1f)] private float BalanceAccuseProbability = 0.07f;
    [SerializeField, Range(0f, 1f)] private float BalanceAgreeProbability = 0.4f;

    [SerializeField, Range(0f, 1f)] private float SelfishAccuseProbability = 0.12f;
    [SerializeField, Range(0f, 1f)] private float SelfishAgreeProbability = 0.8f;

    bool _voteStarted = false;
    PlayerID[] _accusedPlayers = null;

    public override IEnumerator PlayingPhase(AccusationPhase Phase, AIPlayer AI)
    {
        _voteStarted = false;
        Phase.OnAccusationVoteStarted += OnAccusationVoteStarted;

        yield return new WaitForSeconds(AI.Config.ActionDelay);

        bool selfish = AI.GameInstance.ClientPlayerData.Role == PlayerRole.Selfish;
        float AccuseProbability = selfish ? SelfishAccuseProbability : BalanceAccuseProbability;
        float AgreeProbability = selfish ? SelfishAgreeProbability : BalanceAgreeProbability;

        if (AccuseProbability > UnityEngine.Random.value)
        {
            List<PlayerID> ids = new();
            foreach (PlayerID id in Enum.GetValues(typeof(PlayerID)))
            {
                if (id == PlayerID.None) continue;
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
            
            AI.Log("votes " + (vote ? "yes" : "no"));
            Phase.Vote(vote);
        }

    }

    private void OnAccusationVoteStarted(PlayerID[] accusedPlayers)
    {
        _voteStarted = true;
        _accusedPlayers = accusedPlayers;
    }
}
