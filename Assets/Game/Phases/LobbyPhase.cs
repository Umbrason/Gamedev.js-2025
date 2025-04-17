using System.Collections;
using UnityEngine;

public class LobbyPhase : IGamePhase
{
    public GameInstance Game { private get; set; }
    const string StartCommandHeader = "StartGame";
    public IEnumerator OnEnter()
    {
        void OnStartRecieved(NetworkMessage message) => Game.TransitionPhase(new InitGamePhase());
        Game.NetworkChannel.StartListening(StartCommandHeader, OnStartRecieved);
        yield return null;
    }

    public IEnumerator OnExit()
    {
        Game.NetworkChannel.StopListening(StartCommandHeader);
        yield return null;
    }
}