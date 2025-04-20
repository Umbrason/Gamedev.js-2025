using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetitionPhase : IGamePhase
{
    public GameInstance Game { private get; set; }
    public readonly Dictionary<PlayerID, BuildingPetition> Petitions = new();
    const string SubmitPetitionHeader = "SubmitPetition";
    public IEnumerator OnEnter()
    {
        Game.NetworkChannel.StartListening(SubmitPetitionHeader, OnPetitionRecieved);
        yield return null;
    }

    public void OnPetitionRecieved(NetworkMessage message) => Petitions[message.sender] = (BuildingPetition)message.content;

    public IEnumerator Loop()
    {
        yield return new WaitUntil(() => Petitions.Count >= NetworkUtils.playerCount);
        Game.TransitionPhase(new VotePhase(Petitions));
    }

    public IEnumerator OnExit()
    {
        Game.NetworkChannel.StopListening(SubmitPetitionHeader);
        yield return null;
    }

    public bool ClientPetitionSubmitted => Petitions.ContainsKey(Game.ClientID);
    [PlayerAction]
    public void SubmitPetition(BuildingPetition petition)
    {
        Petitions.ContainsKey(Game.ClientID);
        Game.NetworkChannel.BroadcastMessage(SubmitPetitionHeader, petition);
        Petitions.Add(Game.ClientID, petition);
    }
}