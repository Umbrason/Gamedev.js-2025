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
    public Dictionary<PlayerID, int> CurrentVotes { get; private set; } = new();

    public delegate void PetitionChanged();
    public delegate void PetitionDecided(bool success);
    public delegate void Voted(PlayerID id, int vote);
    public event PetitionChanged OnPetitionChanged;
    public event PetitionDecided OnPetitionDecided;
    public event Voted OnVoted;

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
            if (petition == null || petition.Building == Building.None) continue;
            CurrentPetition = petition;
            OnPetitionChanged?.Invoke();

            Game.NetworkChannel.StartListening(SubmitVoteHeader, (message) =>
            {
                int vote = (int)message.content;
                CurrentVotes[message.sender] = vote;
                OnVoted?.Invoke(message.sender, vote);
            });
            yield return new WaitUntil(() => CurrentVotes.Count >= NetworkUtils.playerCount);
            Game.NetworkChannel.StopListening(SubmitVoteHeader);

            var sum = CurrentVotes.Sum(v => v.Value);
            OnPetitionDecided?.Invoke(sum > 0);

            // TODO: animations for vote finished
            yield return Helpers.GetWait(3f);   // wait 3 seconds for animations to finish

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
        }
        // maybe there is a reason this was at the end of the loop?
        CurrentPetition = null;
        OnPetitionChanged?.Invoke();

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