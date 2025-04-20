using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    private string username;
    private string currentRoomCode;
    private int serverPlayerId;
    private bool isHost = false;
    private PlayerID myPlayerID = PlayerID.None;

    private Coroutine pollingCoroutine;
    private float pullingInterval = .25f;

    public string Username { get => username; }
    public string CurrentRoomCode { get => currentRoomCode; }
    public int ServerPlayerId { get => serverPlayerId; }
    public bool IsHost { get => isHost; }
    public PlayerID MyPlayerID { get => myPlayerID; }

    public readonly Queue<INetworkChannel> availableChannels = new(); 

    private Dictionary<PlayerID, INetworkChannel> channels = new();

    public INetworkChannel Channels(PlayerID playerID)
    {
        return channels[playerID];
    }

    public void Initialize(string username, string roomCode, int serverPlayerId, bool isHost, PlayerID playerID)
    {
        this.username = username;
        this.currentRoomCode = roomCode;
        this.serverPlayerId = serverPlayerId;
        this.isHost = isHost;
        this.myPlayerID = playerID;

        channels[playerID] = new ProductionNetwork(playerID);

        availableChannels.Enqueue(channels[playerID]);

        Debug.Log($"[GameNetworkManager] Initialisiert mit Username: {username}, Room: {roomCode}, PlayerID: {serverPlayerId}, Host: {isHost}, GamePlayerID: {playerID}");
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
            yield return new WaitForSeconds(pullingInterval);
        }
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}