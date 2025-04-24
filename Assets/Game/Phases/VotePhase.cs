using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        yield return new WaitUntil(() => Game.NetworkChannel.DistributedRandomDecision(RandomPetitionOrderHeader, ref RandomPetitionIndexResults));
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
            yield return new WaitUntil(() => CurrentVotes.Count >= NetworkUtils.playerCount);
            Game.NetworkChannel.StopListening(SubmitVoteHeader);

            //TODO: actually do something with the vote here
            var sum = CurrentVotes.Sum(v => v.Value);
            OnPetitionDecided?.Invoke(sum > 0);

            if (sum > 0) //construct the greatest building and make the players pay for it
            {
                var allPlayersHaveRequiredResources = CurrentPetition.ResourceSources.All(resourceSource => Game.PlayerData[resourceSource.Key].HasResources(resourceSource.Value));
                if (allPlayersHaveRequiredResources)
                {
                    foreach (var (player, demand) in CurrentPetition.ResourceSources)
                    {
                        var playerData = Game.PlayerData[player];
                        foreach (var (resource, amount) in demand)
                            playerData[resource] -= amount;
                    }
                    Game.PlayerData[CurrentPetition.PlayerID].Island = Game.PlayerData[CurrentPetition.PlayerID].Island.WithBuildings((CurrentPetition.Position, CurrentPetition.Building));
                }
            }
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