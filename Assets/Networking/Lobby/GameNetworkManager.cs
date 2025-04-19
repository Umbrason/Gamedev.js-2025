using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
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

    public void Initialize(string username, string roomCode, int playerId, bool isHost, PlayerID playerID)
    {
        this.username = username;
        this.currentRoomCode = roomCode;
        this.playerId = playerId;
        this.isHost = isHost;
        this.myPlayerID = playerID;

        Debug.Log($"[GameNetworkManager] Initialisiert mit Username: {username}, Room: {roomCode}, PlayerID: {playerId}, Host: {isHost}, GamePlayerID: {playerID}");
        StartPolling();
    }

    public void StartPolling()
    {
        if (pollingCoroutine == null)
            pollingCoroutine = StartCoroutine(PollMessagesLoop());
    }

    public void StopPolling()
    {
        if (pollingCoroutine != null)
        {
            StopCoroutine(pollingCoroutine);
            pollingCoroutine = null;
        }
    }

    private IEnumerator PollMessagesLoop()
    {
        while (true)
        {
            if(Instances.Instance != null)
            Instances.Instance?.AllGameInstances[(int)myPlayerID].NetworkChannel.PollMessages();
            yield return new WaitForSeconds(pollingInterval);
        }
    }
}