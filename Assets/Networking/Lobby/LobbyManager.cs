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
    [SerializeField] private TMP_InputField inputJoinRoomCode;
    [SerializeField] private Button buttonConfirmJoin;
    [SerializeField] private GameObject panelLobby;
    [SerializeField] private TMP_Text textRoomCode;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerEntryPrefab;
    [SerializeField] private Button buttonLeave;
    [SerializeField] private Button buttonStartGame;

    private string serverBaseURL = "https://jamapi.heggie.dev/";

    private string username;
    private string currentRoomCode;
    private int playerId;
    private bool isHost = false;
    private PlayerID myPlayerID = PlayerID.None;

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
        panelJoinRoom.SetActive(false);
        panelLobby.SetActive(true);
        textRoomCode.text = "Room Code: " + currentRoomCode;
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

    IEnumerator SendHeartbeat()
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerId);
        UnityWebRequest www = UnityWebRequest.Post(serverBaseURL + "heartbeat.php", form);
        yield return www.SendWebRequest();
    }

    IEnumerator GetPlayers(string roomCode)
    {
        UnityWebRequest www = UnityWebRequest.Get(serverBaseURL + "get_players.php?room_code=" + roomCode);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;

            try
            {
                PlayerListWrapper list = JsonUtility.FromJson<PlayerListWrapper>(json);
                UpdatePlayerListUI(list.players);

                if (list.game_started)
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            Debug.LogError("Start Game Error: " + www.error);
        }
    }

    void LoadGameScene()
    {
        Debug.Log(myPlayerID);
        GameNetworkManager.Instance.Initialize(username, currentRoomCode, playerId, isHost, myPlayerID);

        AsyncOperation ao = SceneManager.LoadSceneAsync("PlayerGame_Net", LoadSceneMode.Additive);

        ao.completed += _ => { SceneManager.UnloadSceneAsync(gameObject.scene); };
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
    }

    [System.Serializable]
    public class PlayerListWrapper
    {
        public List<Player> players;
        public bool game_started;
    }
}