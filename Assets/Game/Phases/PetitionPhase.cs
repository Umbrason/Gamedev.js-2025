using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetitionPhase : IGamePhase
{
    public GameInstance Game { private get; set; }
    public Dictionary<PlayerID, BuildingPetition> Petitions;
    const string SubmitPetitionHeader = "SubmitPetition";
    public IEnumerator OnEnter()
    {
        yield return null;
    }

    public IEnumerator Loop()
    {
        yield return new WaitUntil(() => Petitions.Count >= 6);
        Game.TransitionPhase(new VotePhase(Petitions));
    }

    public IEnumerator OnExit()
    {
        yield return null;
    }

    void SubmitPetition(BuildingPetition petition)
    {
        Petitions.ContainsKey(Game.ClientID);
        Game.NetworkChannel.BroadcastMessage(SubmitPetitionHeader, petition);
        Petitions.Add(Game.ClientID, petition);
    }
}