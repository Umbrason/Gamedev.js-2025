#define JakobTest

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    private Coroutine pollingCoroutine;
    private float pullingInterval = .5f;

    public readonly Queue<INetworkChannel> availableChannels = new();

    private Dictionary<PlayerID, INetworkChannel> channels = new();

    public INetworkChannel Channels(PlayerID playerID)
    {
        return channels[playerID];
    }

    public void Initialize(string username, string roomCode, int serverPlayerId, bool isHost, PlayerID playerID)
    {
#if JakobTest
        channels[playerID] = new ProductionNetwork(playerID, roomCode, serverPlayerId, username);
#elif UNITY_EDITOR
        channels[playerID] = new LocalDummyNetwork(playerID, username);
#else
        channels[playerID] = new ProductionNetwork(playerID, roomCode, serverPlayerId, username);
#endif

        availableChannels.Enqueue(channels[playerID]);

        Debug.Log($"[GameNetworkManager] Initialisiert mit Username: {username}, Room: {roomCode}, PlayerID: {serverPlayerId}, Host: {isHost}, GamePlayerID: {playerID}");
        StartPulling();
    }

    public void Add_AI(string roomCode, int amount)
    {
        for (int i = NetworkUtils.playerCount - amount; i < NetworkUtils.playerCount; i++)
        {
#if JakobTest
            channels[(PlayerID)i] = new ProductionNetwork((PlayerID)i, roomCode, -1, $"AI_{i}");
#elif UNITY_EDITOR
            channels[(PlayerID)i] = new LocalDummyNetwork((PlayerID)i, $"AI_{i}");
#else
            channels[(PlayerID)i] = new ProductionNetwork((PlayerID)i, roomCode, -1, $"AI_{i}");
#endif
            availableChannels.Enqueue(channels[(PlayerID)i]);
            SceneManager.LoadSceneAsync("AIGame", LoadSceneMode.Additive);
        }
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
            foreach (var channel in channels.Values)
            {
                if (channel is ProductionNetwork network)
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