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

    public Dictionary<string, Action<NetworkMessage>> MessageListeners { get; } = new();
    public Dictionary<string, Queue<NetworkMessage>> MessageBacklog { get; } = new();

    public PlayerID PlayerID => GameNetworkManager.Instance.MyPlayerID;

    private const string BaseUrl = "https://jamapi.heggie.dev/";

    public ProductionNetwork()
    {
        RoomCode = GameNetworkManager.Instance.CurrentRoomCode;
        ServerPlayerID = GameNetworkManager.Instance.PlayerId;
    }

    public void BroadcastMessage(string header, object message)
    {
        UnityEngine.Debug.Log($"Player {Enum.GetName(typeof(PlayerID), GameNetworkManager.Instance.MyPlayerID)} Broadcasts: {message} on channel '{header}'");
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
        form.AddField("sender_id", GameNetworkManager.Instance.PlayerId);
        form.AddField("sender_game_id", (int)GameNetworkManager.Instance.MyPlayerID);
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

    public void PullMessages()
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
                    Recieve(new NetworkMessage(msg.Header, msg.Message, (PlayerID)int.Parse(msg.Sender.ToString())));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON Deserialization Error: " + ex.Message);
            }
        };
    }

    public void Recieve(NetworkMessage message)
    {
        Debug.Log($"Received Message {message.header}, {message.content}, From: {message.sender}");

        if (MessageListeners.ContainsKey(message.header))
        {
            MessageListeners[message.header].Invoke(message);
            return;
        }

        if (!MessageBacklog.ContainsKey(message.header))
            MessageBacklog[message.header] = new Queue<NetworkMessage>();

        MessageBacklog[message.header].Enqueue(message);
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