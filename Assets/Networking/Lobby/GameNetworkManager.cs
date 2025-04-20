using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    public bool useDummyNetwork;

    private string username;
    private string currentRoomCode;
    private int playerId;
    private bool isHost = false;
    private PlayerID myPlayerID = PlayerID.None;

    private Coroutine pollingCoroutine;
    private float pollingInterval = 1.0f;

    public string Username { get => username; }
    public string CurrentRoomCode { get => currentRoomCode; }
    public int PlayerId { get => playerId; }
    public bool IsHost { get => isHost; }
    public PlayerID MyPlayerID { get => myPlayerID; }

    public readonly Queue<INetworkChannel> availableChannels = new(); 

    private Dictionary<PlayerID, INetworkChannel> channels = new();

    public INetworkChannel Channels(PlayerID playerID)
    {
        return channels[playerID];
    }

    public void Initialize(string username, string roomCode, int playerId, bool isHost, PlayerID playerID)
    {
        this.username = username;
        this.currentRoomCode = roomCode;
        this.playerId = playerId;
        this.isHost = isHost;
        this.myPlayerID = playerID;

        channels[playerID] = useDummyNetwork ? new LocalDummyNetwork() : new ProductionNetwork(playerID);

        availableChannels.Enqueue(channels[playerID]);

        Debug.Log($"[GameNetworkManager] Initialisiert mit Username: {username}, Room: {roomCode}, PlayerID: {playerId}, Host: {isHost}, GamePlayerID: {playerID}");
        StartPulling();
    }

    public void StartPulling()
    {
        if (pollingCoroutine == null)
            pollingCoroutine = StartCoroutine(PullMessagesLoop());
    }

    public void StopPulling()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
    }

    private IEnumerator PullMessagesLoop()
    {
        while (true)
        {
            foreach(var channel in channels.Values)
            {
                if(channel is ProductionNetwork network)
                {
                    network.PullMessages();
                }
            }
            yield return new WaitForSeconds(pollingInterval);
        }
    }
}