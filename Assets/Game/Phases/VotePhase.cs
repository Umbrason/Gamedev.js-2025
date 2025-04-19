using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static VotePhase;

public class VotePhase : IGamePhase
{
    public Dictionary<PlayerID, BuildingPetition> Petitions;
    public VotePhase(Dictionary<PlayerID, BuildingPetition> petitions)
    {
        Petitions = petitions;
    }

    public GameInstance Game { private get; set; }
    const string RandomPetitionOrderHeader = "RndPetitionOrder";
    private List<PlayerID> PetitionOrder;
    public BuildingPetition CurrentPetition { get; private set; }
    private Dictionary<PlayerID, int> CurrentVotes = new();

    public delegate void PetitionChanged();
    public delegate void PetitionDecided(bool success);
    public event PetitionChanged OnPetitionChanged;
    public event PetitionDecided OnPetitionDecided;

    public IEnumerator OnEnter()
    {
        var RandomPetitionIndexResults = (Dictionary<PlayerID, float>)null;
        yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(Game.ClientID, RandomPetitionOrderHeader, ref RandomPetitionIndexResults));
        PetitionOrder = RandomPetitionIndexResults.OrderBy(pair => pair.Value).Select(pair => pair.Key).ToList();
    }

    public IEnumerator Loop()
    {
        foreach (var playerID in PetitionOrder)
        {
            var petition = Petitions[playerID];
            if (petition == null) continue;

            CurrentPetition = petition;
            OnPetitionChanged?.Invoke();

            Game.NetworkChannel.StartListening(SubmitVoteHeader, (message) => CurrentVotes[message.sender] = (int)message.content);
            yield return new WaitUntil(() => CurrentVotes.Count >= 6);
            Game.NetworkChannel.StopListening(SubmitVoteHeader);

            //TODO: actually do something with the vote here
            OnPetitionDecided?.Invoke(CurrentVotes.Sum(v => v.Value) > 0);


            CurrentVotes.Clear();
            CurrentPetition = null;
            OnPetitionChanged?.Invoke();
        }
        Game.TransitionPhase(new BuildPhase());
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }

    const string SubmitVoteHeader = "SubmitPetitionVote";
    [PlayerAction]
    public void SubmitVote(int vote)
    {
        CurrentVotes[Game.ClientID] = vote;
        Game.NetworkChannel.BroadcastMessage(SubmitVoteHeader, vote);
    }
}