using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccusationPhase : IGamePhase, ITimedPhase
{
    public GameInstance Game { private get; set; }

    public float TimeRemaining => Mathf.Max(0, Duration - (Time.unscaledTime - SubphaseStart));
    private float SubphaseStart;
    public float Duration { get; private set; }

    public IEnumerator OnEnter()
    {
        yield return null;
    }


    public float AccusationDuration = 10f;
    public float AccusationVoteDuration = 30f;
    bool accusationMade = false;
    public void Accuse(PlayerID[] Accusation = null)
    {
        if (accusationMade) return;
        accusationMade = true;
        Accusations[Game.ClientID] = Accusation;
        Game.NetworkChannel.BroadcastMessage(AccusationMade, Accusation);
    }

    const string AccusationMade = "AccusationMade";
    const string AccusationsOrder = "AccusationsOrder";
    Dictionary<PlayerID, PlayerID[]> Accusations = new();
    Dictionary<PlayerID, bool> currentVotes = new();
    public IEnumerator Loop()
    {
        SubphaseStart = Time.unscaledTime;
        Duration = AccusationDuration;
        if(Duration <= 0) 
        yield return new WaitUntil(() => Accusations.Count == NetworkUtils.playerCount);

        var accusationOrder = new Dictionary<PlayerID, float>();
        yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(AccusationsOrder, ref accusationOrder));

        foreach (var (accuser, AccusedPlayers) in Accusations.OrderBy(p => accusationOrder[p.Key]))
        {
            Duration = AccusationVoteDuration;
            SubphaseStart = Time.unscaledTime;
            if (AccusedPlayers == null) continue;

            void OnVoteRecieved(NetworkMessage message) => currentVotes[message.sender] = (bool)message.content;
            Game.NetworkChannel.StartListening(ShareVoteHeader, OnVoteRecieved);

            if (AccusedPlayers.Contains(Game.ClientID)) Vote(true); //accused players can't vote

            yield return new WaitUntil(() => currentVotes.Count >= NetworkUtils.playerCount);

            if (currentVotes.Values.All(v => v))
                Game.TransitionPhase(new GameOverPhase(AccusedPlayers.All(playerID => Game.PlayerData[playerID].Role == PlayerRole.Selfish) ? PlayerRole.Balanced : PlayerRole.Selfish));

            currentVotes.Clear();
        }
        Game.TransitionPhase(new RoundTransitionPhase());
    }

    public IEnumerator OnExit()
    {
        Game.NetworkChannel.StopListening(ShareVoteHeader);
        yield return null;
    }

    const string ShareVoteHeader = "ShareAccusationVote";
    bool canVote = false;

    [PlayerAction]
    public void Vote(bool agreeWithAccusation)
    {
        if (!canVote) return;
        canVote = false;
        Game.NetworkChannel.BroadcastMessage(ShareVoteHeader, agreeWithAccusation);
        currentVotes[Game.ClientID] = agreeWithAccusation;
    }


    public bool CanSkip() => false;
    public void Skip() { }
}