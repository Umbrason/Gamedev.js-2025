using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;

public class ProductionNetwork : INetworkChannel
{
    public string RoomCode { get; private set; }
    public int ServerPlayerID { get; private set; }

    private static readonly Dictionary<PlayerID, INetworkChannel> NetworkParticipants = new();
    public Dictionary<string, Action<NetworkMessage>> MessageListeners { get; } = new();
    public Dictionary<string, Queue<NetworkMessage>> MessageBacklog { get; } = new();

    public PlayerID PlayerID { get; set; }

    private const string BaseUrl = "https://jamapi.heggie.dev/";

    public ProductionNetwork()
    {
        PlayerID = (PlayerID)NetworkParticipants.Count;
        NetworkParticipants.Add(PlayerID, this);

        RoomCode = GameNetworkManager.Instance.CurrentRoomCode;
        ServerPlayerID = GameNetworkManager.Instance.PlayerId;
    }

    public void BroadcastMessage(string header, object message)
    {
        UnityEngine.Debug.Log($"Player {Enum.GetName(typeof(PlayerID), PlayerID)} Broadcasts: {message} on channel '{header}'");
        SendToServer("broadcastMessage.php", header, message, null);
    }

    public void SendMessage(string header, object message, PlayerID receiver)
    {
        SendToServer("sendMessage.php", header, message, receiver.ToString());
    }

    private void SendToServer(string endpoint, string header, object message, string? receiver)
    {
        WWWForm form = new();
        form.AddField("room_code", RoomCode);
        form.AddField("sender_id", ServerPlayerID);
        form.AddField("header", header);

        string messageString;

        if (message is float floatVal)
        {
            messageString = floatVal.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            messageString = message.ToString();
        }

        form.AddField("message", messageString);

        if (receiver != null)
            form.AddField("receiver_id", receiver);

        UnityWebRequest www = UnityWebRequest.Post(BaseUrl + endpoint, form);
        www.SendWebRequest().completed += _ =>
        {
            if (www.result != UnityWebRequest.Result.Success)
                Debug.LogError("Network Error: " + www.error);
            else
                Debug.Log("Message sent: " + www.downloadHandler.text);
        };
    }

    public void PollMessages()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{BaseUrl}getMessages.php?room_code={RoomCode}&player_id={ServerPlayerID}");
        www.SendWebRequest().completed += _ =>
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Message Polling Error: " + www.error);
                return;
            }

            string json = www.downloadHandler.text;

            try
            {
                MessageData[] messages = JsonHelper.FromJson<MessageData>(json);

                foreach (var msg in messages)
                {
                    if (NetworkParticipants.TryGetValue(PlayerID, out var networkChannel))
                    {
                        networkChannel.Recieve(new NetworkMessage(msg.Header, msg.Message, PlayerID.Zero));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON Deserialization Error: " + ex.Message);
            }
        };
    }

    public bool TryGetNextBacklogMessage(string header, out NetworkMessage message)
    {
        if (MessageBacklog.TryGetValue(header, out var queue) && queue.Count > 0)
        {
            message = queue.Dequeue();
            return true;
        }

        message = new NetworkMessage();
        return false;
    }
}

[System.Serializable]
public class MessageData
{
    public string Header;
    public string Message;
    public int Sender;
}

public static class JsonHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new() { Items = array };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
}