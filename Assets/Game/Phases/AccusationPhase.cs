using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccusationPhase : IGamePhase, ITimedPhase
{
    public GameInstance Game { private get; set; }

    public float TimeRemaining => Mathf.Max(0, Duration - (Time.unscaledTime - startTime));
    public float startTime { get; set; }
    public float Duration { get; private set; } = 1f;

    public event Action<PlayerID[]> OnAccusationVoteStarted;
    public IEnumerator OnEnter()
    {
        yield return null;
    }


    public float AccusationDuration = 10f;
    public float AccusationVoteDuration = 30f;
    public void Accuse(PlayerID[] Accusation = null)
    {
        if (Accusation == null) Accusation = new PlayerID[0];
        if (Accusations.ContainsKey(Game.ClientID)) return;
        Accusations[Game.ClientID] = Accusation;
        Game.NetworkChannel.BroadcastMessage(AccusationMade, Accusation);
    }

    const string AccusationMade = "AccusationMade";
    const string AccusationsOrder = "AccusationsOrder";
    Dictionary<PlayerID, PlayerID[]> Accusations = new();
    Dictionary<PlayerID, bool> currentVotes = new();
    public IEnumerator Loop()
    {
        startTime = Time.unscaledTime;
        Duration = AccusationDuration;
        yield return new WaitUntil(() => TimeRemaining <= 0 || Accusations.ContainsKey(Game.ClientID));
        if (!Accusations.ContainsKey(Game.ClientID)) Accuse(null);
        void OnMessageRecieved(NetworkMessage message)
        {
            Accusations[message.sender] = (PlayerID[])message.content;
            if (Accusations.Count == NetworkUtils.playerCount)
                Game.NetworkChannel.StopListening(AccusationMade);
        }
        Game.NetworkChannel.StartListening(AccusationMade, OnMessageRecieved);
        yield return new WaitUntil(() => Accusations.Count == NetworkUtils.playerCount);

        var accusationOrder = (Dictionary<PlayerID, float>)null;
        yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(AccusationsOrder, ref accusationOrder));

        foreach (var (accuser, AccusedPlayers) in Accusations.OrderBy(p => accusationOrder[p.Key]))
        {
            Duration = AccusationVoteDuration;
            startTime = Time.unscaledTime;
            if ((AccusedPlayers?.Length ?? 0)  == 0) continue;
            canVote = true;
            OnAccusationVoteStarted?.Invoke(AccusedPlayers);
            void OnVoteRecieved(NetworkMessage message) => currentVotes[message.sender] = (bool)message.content;
            Game.NetworkChannel.StartListening(ShareVoteHeader, OnVoteRecieved);
            if (AccusedPlayers.Contains(Game.ClientID)) Vote(true); //accused players can't vote
            yield return new WaitUntil(() => TimeRemaining <= 0 || currentVotes.ContainsKey(Game.ClientID));
            if (!currentVotes.ContainsKey(Game.ClientID)) Vote(false);
            yield return new WaitUntil(() => currentVotes.Count >= NetworkUtils.playerCount);
            Game.NetworkChannel.StopListening(ShareVoteHeader);

            if (currentVotes.Values.All(v => v))
                Game.TransitionPhase(new GameOverPhase(AccusedPlayers.All(playerID => Game.PlayerData[playerID].Role == PlayerRole.Selfish) ? PlayerRole.Balanced : PlayerRole.Selfish));

            currentVotes.Clear();
        }
        Game.TransitionPhase(new RoundTransitionPhase());
    }

    public IEnumerator OnExit()
    {
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