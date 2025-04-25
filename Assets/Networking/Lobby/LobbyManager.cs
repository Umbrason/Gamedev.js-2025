using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;
using System;

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

    public const string serverBaseURL = "https://jamapi.heggie.dev/";

    private string username;
    private string currentRoomCode;
    private int playerId;
    private bool isHost = false;
    private PlayerID myPlayerID = PlayerID.None;

    PlayerListWrapper playerList;

    bool gameLoaded;

    public string CurrentRoomCode { get => currentRoomCode; set => currentRoomCode = value; }
    public int PlayerId { get => playerId; set => playerId = value; }

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

        //inputUsername.text = "_default";
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
            Debug.Log("Create Room Response: " + www.downloadHandler.text);
            RoomJoinResponse response = JsonUtility.FromJson<RoomJoinResponse>(www.downloadHandler.text);
            currentRoomCode = response.room_code;
            playerId = response.player_id;
            isHost = true;
            OpenLobbyPanel();

            GUIUtility.systemCopyBuffer = currentRoomCode;
        }
        else
        {
            Debug.LogError("Create Room Error: " + www.error);
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
            Debug.LogError("Join Room Error: " + www.downloadHandler.text);
        }
    }

    void OpenLobbyPanel()
    {
        //Fade between activations.
        SceneFader.Instance.FadeAction(() => {
            mainRoom.SetActive(false);
            panelJoinRoom.SetActive(false);
            panelLobby.SetActive(true);
        });

        //Get room code
        roomCodeAsCopyableIF.text = currentRoomCode.ToString();
        buttonStartGame.gameObject.SetActive(isHost);

        StartCoroutine(UpdatePlayerListLoop());
    }

    IEnumerator UpdatePlayerListLoop()
    {
        while (panelLobby.activeSelf)
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

    /*
    IEnumerator GetPlayers(string roomCode)
    {
        UnityWebRequest www = UnityWebRequest.Get(serverBaseURL + "get_players.php?room_code=" + roomCode);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;

            try
            {
                playerList = JsonUtility.FromJson<PlayerListWrapper>(json);
                UpdatePlayerListUI(playerList.players);

                if (playerList.game_started && !gameLoaded)
                {
                    LoadGameScene();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON Parsing Error: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("GetPlayers Error: " + www.error);
        }
    }*/

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
                Debug.LogError(errorMsg);
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
        foreach (Transform child in playerListContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count; i++)
        {
            Player player = players[i];
            GameObject entry = Instantiate(playerEntryPrefab, playerListContainer);
            entry.GetComponentInChildren<TMP_Text>().text = player.player_name;

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
        //StartCoroutine(OnGameStarteClicked_async());
    }

    /*
    IEnumerator OnGameStarteClicked_async()
    {
       
        if (isHost)
        {
            for (int i = NetworkUtils.playerCount - playerList.players.Count; i < NetworkUtils.playerCount; i++)
            {
                yield return StartCoroutine(RegisterAiOnServer($"AI_{i}", currentRoomCode, (PlayerID)i));
            }
        }
        
        StartCoroutine(SetRoomStarted());
    }*/

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
            Debug.LogError("Start Game Error: " + www.error);
        }
    }

    void LoadGameScene()
    {
        if (!gameLoaded)
        {
            gameLoaded = true;
            Debug.Log(myPlayerID);
            GameNetworkManager.Instance.Initialize(username, currentRoomCode, playerId, isHost, myPlayerID);

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

    //TODO VALIDATE SUCCESS
    /*public IEnumerator RegisterAiOnServer(string userName, string roomCode, PlayerID playerID)
    {
        WWWForm form = new WWWForm();
        form.AddField("room_code", roomCode);
        form.AddField("player_name", userName);

        UnityWebRequest www = UnityWebRequest.Post(LobbyManager.serverBaseURL + "join_room.php", form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success && !www.downloadHandler.text.Contains("error"))
        {
            RoomJoinResponse response = JsonUtility.FromJson<RoomJoinResponse>(www.downloadHandler.text);
            int playerID_fromServer = response.player_id;

            GameNetworkManager.Instance.availableAIChannels.Enqueue(new ProductionNetwork(playerID, roomCode, playerID_fromServer));

            Debug.Log($"[AI NETWORK] Initialisiert mit Username: {username}, Room: {roomCode}, PlayerID: {playerID_fromServer}, Host: {isHost}, GamePlayerID: {playerID}");
        }
        else
        {
            Debug.LogError("Join Room Error: " + www.downloadHandler.text);
        }
    }*/

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