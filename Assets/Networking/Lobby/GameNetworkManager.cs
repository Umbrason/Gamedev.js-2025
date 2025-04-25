//#define JakobTest
//#define NetworkDebug

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Networking;
using System;
using System.IO;
using static LobbyManager;
using UnityEngine.UI;

public class GameNetworkManager : Singleton<GameNetworkManager>
{
    private Coroutine pollingCoroutine;
    private float pullingInterval = .5f;
    private float checkPlayerConnectedInterval = 5f;

    string roomCode;

    public readonly Queue<INetworkChannel> availableChannels = new();

    private Dictionary<PlayerID, INetworkChannel> channels = new();

    List<Player> allTimePlayers = new List<Player>();

    [SerializeField]
    private GameObject selfDisconnectedPanel, otherPlayerDisconnectedPanel;
    [SerializeField]
    private Button backToMainMenu_selfDisconnect, backToMainMenu_otherPlayerDisconnect;
    [SerializeField]
    private TMPro.TMP_Text playerDisconnctedInformationText;

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
#if NetworkDebug
        Debug.Log($"[GameNetworkManager] Initialized with Username: {username}, Room: {roomCode}, PlayerID: {serverPlayerId}, Host: {isHost}, GamePlayerID: {playerID}");
#endif
        StartPulling();

        StartCoroutine(CheckPlayerConnectedLoop());

        backToMainMenu_selfDisconnect.onClick.AddListener(GoBackToMainMenu);
        backToMainMenu_otherPlayerDisconnect.onClick.AddListener(GoBackToMainMenu);
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

        SoundAndMusicController.Instance.EnsureSingleAudioListener();
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
            PullAllMessages();
            yield return new WaitForSeconds(pullingInterval);
        }
    }

    private IEnumerator CheckPlayerConnectedLoop()
    {
        while (true)
        {
            StartCoroutine(GetPlayers());
            yield return new WaitForSeconds(checkPlayerConnectedInterval);
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
#if NetworkDebug
                Debug.LogError("Multi-Pull Error: " + www.error);
#endif
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
#if NetworkDebug
                        Debug.LogError($"Unknown type: {msg.MessageType}");
#endif
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
#if NetworkDebug
                        Debug.LogError($"Failed to deserialize: {msg.Message}");
#endif
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
#if NetworkDebug
                        Debug.LogWarning($"No channel found for Receiver {msg.Receiver}");
#endif
                    }
                }
            }
            catch (Exception ex)
            {
#if NetworkDebug
                Debug.LogError("Deserialization error: " + ex.Message);
#endif
            }
        };
    }

    IEnumerator GetPlayers()
    {
        int retryCount = 0;
        int maxRetries = 3;
        bool success = false;

        while (retryCount < maxRetries && !success)
        {
            bool done = false;

            yield return StartCoroutine(LobbyManager.FetchPlayerList(
                roomCode,
                (fetchedPlayerList) =>
                {
                    PlayerListWrapper newPlayerList = fetchedPlayerList;

                    if (newPlayerList != null)
                    {
                        // Speichere alle neuen Spieler in allTimePlayers
                        foreach (var newPlayer in newPlayerList.players)
                        {
                            if (!allTimePlayers.Any(p => p.player_id == newPlayer.player_id))
                            {
                                allTimePlayers.Add(newPlayer);
                            }
                        }

                        // Finde alle Spieler, die jemals da waren, aber aktuell fehlen
                        List<Player> missingPlayers = allTimePlayers
                            .Where(savedPlayer => !newPlayerList.players.Any(p => p.player_id == savedPlayer.player_id))
                            .ToList();

                        if (missingPlayers.Count > 0)
                        {
                            otherPlayerDisconnectedPanel.SetActive(true);

                            string infoText = string.Join(", ", missingPlayers.Select(p => p.player_name));
                            string verb = missingPlayers.Count == 1 ? "has" : "have";
                            playerDisconnctedInformationText.text = $"{infoText} {verb} lost the connection to the server.";
                        }
                        else
                        {
                            otherPlayerDisconnectedPanel.SetActive(false);
                        }
                    }

                    success = true;
                    done = true;
                },
                (errorMsg) =>
                {
#if NetworkDebug
                    Debug.LogError($"{errorMsg}");
#endif
                    retryCount++;
                    done = true;
                }
            ));

            while (!done)
            {
                yield return null;
            }

            if (!success && retryCount < maxRetries)
            {
#if NetworkDebug
                Debug.LogWarning($"Reconnecting {retryCount}/{maxRetries}...");
#endif
                yield return new WaitForSeconds(checkPlayerConnectedInterval);
            }
        }

        if (!success)
        {
            selfDisconnectedPanel.SetActive(true);
        }
        else
        {
            selfDisconnectedPanel.SetActive(false);
        }
    }

    private void GoBackToMainMenu()
    {
        SceneFader.Instance.FadeToScene("Main Menu");
        Destroy(gameObject);
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}