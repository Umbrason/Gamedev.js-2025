using System.Collections;

public class LobbyPhase : IGamePhase
{
    public GameInstance Game { private get; set; }

    const string StartCommandHeader = "StartGame";
    const string PlayerConnectedHeader = "PlayerConnected";
    const string PlayerIDAssignmentHeader = "PlayerIDAssignment";
    public IEnumerator OnEnter()
    {
        void OnStartRecieved(NetworkMessage message) => Game.TransitionPhase(new InitGamePhase());
        Game.NetworkChannel.StartListening(StartCommandHeader, OnStartRecieved);

        Game.NetworkChannel.BroadcastMessage(PlayerConnectedHeader, null);

        void OnPlayerIDAssignmentRecieved(NetworkMessage message) => Game.ClientID = (PlayerID)message.content;
        Game.NetworkChannel.StartListening(PlayerIDAssignmentHeader, OnPlayerIDAssignmentRecieved);
        yield return null;
    }

    public IEnumerator OnExit()
    {
        Game.NetworkChannel.StopListening(StartCommandHeader);
        yield return null;
    }
}