#define JakobTest

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Networking;
using System;
using System.IO;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    private Coroutine pollingCoroutine;
    private float pullingInterval = .5f;

    string roomCode;

    public readonly Queue<INetworkChannel> availableChannels = new();

    private Dictionary<PlayerID, INetworkChannel> channels = new();

    public INetworkChannel Channels(PlayerID playerID)
    {
        return channels[playerID];
    }

    public void Initialize(string username, string roomCode, int serverPlayerId, bool isHost, PlayerID playerID)
    {
        this.roomCode = roomCode;

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

    /*
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
    }*/

    private IEnumerator PullMessagesLoop()
    {
        while (true)
        {
            PullAllMessages();
            yield return new WaitForSeconds(pullingInterval);
        }
    }

    public void PullAllMessages()
    {
        string allIDs = string.Join(",", channels.Values
            .OfType<ProductionNetwork>()
            .Select(n => ((int)n.PlayerID).ToString()));

        string url = $"{ProductionNetwork.BaseUrl}getMessages_v3.php?room_code={roomCode}&player_game_ids={allIDs}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SendWebRequest().completed += _ =>
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Multi-Pull Error: " + www.error);
                return;
            }

            string json = www.downloadHandler.text;
            try
            {
                MessageData[] messages = JsonHelper.FromJson<MessageData>(json);

                foreach (var msg in messages)
                {
                    Type type = Type.GetType(msg.MessageType);
                    if (type == null)
                    {
                        Debug.LogError($"Unknown type: {msg.MessageType}");
                        continue;
                    }

                    var ms = new MemoryStream();
                    var sw = new StreamWriter(ms);
                    sw.Write(msg.Message);
                    sw.Flush();
                    ms.Position = 0;
                    var sr = new StreamReader(ms);
                    var yAMLD = new YAMLDeserializer(sr);

                    object deserialized = ReflectionSerializer.DeserializeField(type, "", yAMLD);

                    if (deserialized == null)
                    {
                        Debug.LogError($"Failed to deserialize: {msg.Message}");
                        continue;
                    }

                    if (channels.Values.OfType<ProductionNetwork>().FirstOrDefault(
                            ch => ((int)ch.PlayerID).ToString() == msg.Receiver
                        ) is ProductionNetwork receiver)
                    {
                        receiver.Recieve(new NetworkMessage(msg.Header, deserialized, (PlayerID)int.Parse(msg.Sender)));
                    }
                    else
                    {
                        Debug.LogWarning($"No channel found for Receiver {msg.Receiver}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Deserialization error: " + ex.Message);
            }
        };
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}