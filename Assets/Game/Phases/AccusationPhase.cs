using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AccusationPhase : IGamePhase
{
    public GameInstance Game { private get; set; }

    public readonly IReadOnlyCollection<PlayerID> AccusedPlayers;
    private readonly Dictionary<PlayerID, bool> votes = new();

    public AccusationPhase(PlayerID[] accusedPlayers)
    {
        AccusedPlayers = accusedPlayers;
    }

    public IEnumerator OnEnter()
    {
        void OnVoteRecieved(NetworkMessage message) => votes[message.sender] = (bool)message.content;
        Game.NetworkChannel.StartListening(ShareVoteHeader, OnVoteRecieved);
        if(AccusedPlayers.Contains(Game.ClientID)) Vote(true); //accused players can't vote
        yield return null;
    }

    public IEnumerator Loop()
    {
        yield return new WaitUntil(() => votes.Count >= NetworkUtils.playerCount);
        if (votes.Values.All(v => v))
            Game.TransitionPhase(new GameOverPhase(AccusedPlayers.All(playerID => Game.PlayerData[playerID].Role == PlayerRole.Selfish) ? PlayerRole.Balanced : PlayerRole.Selfish));
        else Game.TransitionPhase(null); //TODO: dunno yet where this should lead back to.
    }

    public IEnumerator OnExit()
    {
        Game.NetworkChannel.StopListening(ShareVoteHeader);
        yield return null;
    }

    const string ShareVoteHeader = "ShareAccusationVote";
    [PlayerAction]
    public void Vote(bool agreeWithAccusation)
    {
        Game.NetworkChannel.BroadcastMessage(ShareVoteHeader, agreeWithAccusation);
        votes[Game.ClientID] = agreeWithAccusation;
    }
}