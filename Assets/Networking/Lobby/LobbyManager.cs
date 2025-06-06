//#define NetworkDebug

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class LobbyManager : Singleton<LobbyManager>
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private Button buttonCreate;
    [SerializeField] private Button buttonJoin;
    [SerializeField] private GameObject panelJoinRoom;
    [SerializeField] private Button buttonConfirmJoin;
    [SerializeField] private GameObject panelLobby;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private Button buttonLeave;
    [SerializeField] private Button buttonStartGame;
    [SerializeField] private GameObject mainRoom;

    [Header("RoomCode Refs")]
    [SerializeField] private TMP_InputField roomCodeAsCopyableIF;
    [SerializeField] private TMP_InputField inputJoinRoomCode;

    public const string serverBaseURL = "https://api-sylvanisspirits.heggie.dev/";

    private string username;
    private string currentRoomCode;
    private int playerId;
    private bool isHost = false;
    private PlayerID myPlayerID = PlayerID.None;

    private Coroutine playerListCoroutine;

    PlayerListWrapper playerList;
    private Dictionary<int, GameObject> currentPlayerEntries = new Dictionary<int, GameObject>();

    bool gameLoaded;

    public string CurrentRoomCode { get => currentRoomCode; set => currentRoomCode = value; }
    public int PlayerId { get => playerId; set => playerId = value; }

    const string UserNamePlayerPrefsKey = "Username";
    private void Start()
    {
        buttonCreate.onClick.AddListener(OnCreateClicked);
        buttonJoin.onClick.AddListener(() => panelJoinRoom.SetActive(true));
        buttonConfirmJoin.onClick.AddListener(OnJoinConfirmed);
        buttonLeave.onClick.AddListener(LeaveRoom);
        buttonStartGame.onClick.AddListener(OnStartGameClicked);
        buttonStartGame.gameObject.SetActive(false);

        panelLobby.SetActive(false);
        panelJoinRoom.SetActive(false);
        inputUsername.SetTextWithoutNotify(PlayerPrefs.GetString(UserNamePlayerPrefsKey));
        inputUsername.onSubmit.AddListener(value => PlayerPrefs.SetString(UserNamePlayerPrefsKey, value.Trim()));
        inputUsername.onEndEdit.AddListener(value => PlayerPrefs.SetString(UserNamePlayerPrefsKey, value.Trim()));
    }

    void OnCreateClicked()
    {
        username = inputUsername.text.Trim();
        if (string.IsNullOrEmpty(username)) return;

        StartCoroutine(CreateRoom(username));
    }

    void OnJoinConfirmed()
    {
        username = inputUsername.text.Trim();
        string roomCode = inputJoinRoomCode.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roomCode)) return;

        StartCoroutine(JoinRoom(roomCode, username));
    }

    IEnumerator CreateRoom(string user)
    {
        WWWForm form = new WWWForm();
        form.AddField("host_name", user);

        UnityWebRequest www = UnityWebRequest.Post(serverBaseURL + "create_room.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            #if NetworkDebug
            Debug.Log("Create Room Response: " + www.downloadHandler.text);
            #endif
            RoomJoinResponse response = JsonUtility.FromJson<RoomJoinResponse>(www.downloadHandler.text);
            currentRoomCode = response.room_code;
            playerId = response.player_id;
            isHost = true;
            OpenLobbyPanel();

            GUIUtility.systemCopyBuffer = currentRoomCode;
        }
        else
        {
            #if NetworkDebug
            Debug.LogError("Create Room Error: " + www.error);
            #endif
        }
    }

    IEnumerator JoinRoom(string roomCode, string user)
    {
        WWWForm form = new WWWForm();
        form.AddField("room_code", roomCode);
        form.AddField("player_name", user);

        UnityWebRequest www = UnityWebRequest.Post(serverBaseURL + "join_room.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success && !www.downloadHandler.text.Contains("error"))
        {
            RoomJoinResponse response = JsonUtility.FromJson<RoomJoinResponse>(www.downloadHandler.text);
            currentRoomCode = roomCode;
            playerId = response.player_id;
            isHost = false;
            OpenLobbyPanel();
        }
        else
        {
#if NetworkDebug
            Debug.LogError("Join Room Error: " + www.downloadHandler.text);
#endif
        }
    }

    void OpenLobbyPanel()
    {
        //Fade between activations.
        SceneFader.Instance.FadeAction(() =>
        {
            mainRoom.SetActive(false);
            panelJoinRoom.SetActive(false);
            panelLobby.SetActive(true);
        });

        //Get room code
        roomCodeAsCopyableIF.text = currentRoomCode.ToString();
        buttonStartGame.gameObject.SetActive(isHost);

        playerListCoroutine = StartCoroutine(UpdatePlayerListLoop());
    }

    IEnumerator UpdatePlayerListLoop()
    {
        while (true)
        {
            StartCoroutine(SendHeartbeat());
            yield return GetPlayers(currentRoomCode);
            yield return new WaitForSeconds(1f);
        }
    }

    public IEnumerator SendHeartbeat()
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        UnityWebRequest www = UnityWebRequest.Post(serverBaseURL + "heartbeat.php", form);
        yield return www.SendWebRequest();
    }

    IEnumerator GetPlayers(string roomCode)
    {
        yield return StartCoroutine(FetchPlayerList(
            roomCode,
            (fetchedPlayerList) =>
            {
                playerList = fetchedPlayerList;
                UpdatePlayerListUI(playerList.players);

                if (playerList.game_started && !gameLoaded)
                {
                    LoadGameScene();
                }
            },
            (errorMsg) =>
            {
#if NetworkDebug
                Debug.LogError(errorMsg);
#endif
            }
        ));
    }

    public static IEnumerator FetchPlayerList(string roomCode, Action<PlayerListWrapper> onSuccess, Action<string> onError)
    {
        UnityWebRequest www = UnityWebRequest.Get(serverBaseURL + "get_players.php?room_code=" + roomCode);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;

            try
            {
                PlayerListWrapper playerList = JsonUtility.FromJson<PlayerListWrapper>(json);
                onSuccess?.Invoke(playerList);
            }
            catch (Exception ex)
            {
                onError?.Invoke("JSON Parsing Error: " + ex.Message);
            }
        }
        else
        {
            onError?.Invoke("GetPlayers Error: " + www.error);
        }
    }

    void UpdatePlayerListUI(List<Player> players)
    {
        HashSet<int> newPlayerIds = new HashSet<int>(players.Select(p => p.player_id));

        var oldPlayerIds = currentPlayerEntries.Keys.ToList();
        foreach (int oldId in oldPlayerIds)
        {
            if (!newPlayerIds.Contains(oldId))
            {
                Destroy(currentPlayerEntries[oldId]);
                currentPlayerEntries.Remove(oldId);
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];

            if (currentPlayerEntries.TryGetValue(player.player_id, out GameObject entry))
            {
                TMP_Text text = entry.GetComponentInChildren<TMP_Text>();
                if (text != null && text.text != player.player_name)
                {
                    text.text = player.player_name;
                }
            }
            else
            {
                GameObject newEntry = Instantiate(playerEntryPrefab, playerListContainer);
                newEntry.GetComponentInChildren<TMP_Text>().text = player.player_name;
                currentPlayerEntries[player.player_id] = newEntry;
            }

            if (player.player_id == this.playerId)
            {
                myPlayerID = (PlayerID)i;
            }
        }
    }

    public void LeaveRoom()
    {
        StartCoroutine(LeaveRoomCoroutine());
    }

    IEnumerator LeaveRoomCoroutine()
    {
        if (playerListCoroutine != null)
        {
            StopCoroutine(playerListCoroutine);
            playerListCoroutine = null;
        }

        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);

        UnityWebRequest www = UnityWebRequest.Post(serverBaseURL + "leave_room.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            panelLobby.SetActive(false);
            SceneFader.Instance.FadeToScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnStartGameClicked()
    {
        StartCoroutine(SetRoomStarted());
    }

    IEnumerator SetRoomStarted()
    {
        WWWForm form = new WWWForm();
        form.AddField("room_code", currentRoomCode);
        UnityWebRequest www = UnityWebRequest.Post(serverBaseURL + "start_game.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            LoadGameScene();
        }
        else
        {
#if NetworkDebug
            Debug.LogError("Start Game Error: " + www.error);
#endif
        }
    }

    void LoadGameScene()
    {
        if (!gameLoaded)
        {
            gameLoaded = true;

            for (int i = 0; i < playerList.players.Count; i++)
            {
                playerList.players[i].player_gameID = i;
            }

            myPlayerID = (PlayerID)playerList.players.First(p => p.player_id == playerId).player_gameID;

            GameNetworkManager.Instance.Initialize(username, currentRoomCode, playerId, isHost, myPlayerID, playerList.players);

            AsyncOperation ao = SceneManager.LoadSceneAsync("PlayerGame", LoadSceneMode.Additive);
            ao.completed += _ =>
            {
                if (isHost)
                {
                    GameNetworkManager.Instance.Add_AI(currentRoomCode, NetworkUtils.playerCount - playerList.players.Count);
                }
                SceneManager.UnloadSceneAsync(gameObject.scene);
            };
        }

        SoundAndMusicController.Instance.EnsureSingleAudioListener();
    }


    [System.Serializable]
    public class RoomJoinResponse
    {
        public string room_code;
        public int player_id;
    }

    [System.Serializable]
    public class Player
    {
        public int player_id;
        public string player_name;
        public int player_gameID;
    }

    [System.Serializable]
    public class PlayerListWrapper
    {
        public List<Player> players;
        public bool game_started;
    }
}