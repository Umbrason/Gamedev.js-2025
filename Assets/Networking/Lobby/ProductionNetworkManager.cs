using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Networking;
using System.Globalization;
using Newtonsoft.Json;
using System.IO;

public class ProductionNetwork : INetworkChannel
{
    public string RoomCode { get; private set; }
    public int ServerPlayerID { get; private set; }

    public Dictionary<string, Action<NetworkMessage>> MessageListeners { get; } = new();
    public Dictionary<string, Queue<NetworkMessage>> MessageBacklog { get; } = new();

    public PlayerID PlayerID { get; }

    private const string BaseUrl = "https://jamapi.heggie.dev/";

    public ProductionNetwork(PlayerID playerID)
    {
        RoomCode = GameNetworkManager.Instance.CurrentRoomCode;
        ServerPlayerID = GameNetworkManager.Instance.ServerPlayerId;

        PlayerID = playerID;
    }

    public void SendMessage(string header, object message, PlayerID receiver)
    {
        SendToServer("sendMessage.php", header, message, receiver.ToString());
    }

    public void BroadcastMessage(string header, object message)
    {
        SendToServer("broadcastMessage.php", header, message, null);
    }

    private void SendToServer(string endpoint, string header, object message, string? receiver)
    {
        WWWForm form = new();
        form.AddField("room_code", RoomCode);
        form.AddField("sender_id", ServerPlayerID);
        form.AddField("sender_game_id", (int)PlayerID);
        form.AddField("header", header);

        MemoryStream ms = new();
        StreamWriter sw = new(ms);
        YAMLSerializer yAMLSerializer = new(sw);
        ReflectionSerializer.SerializeField(message.GetType(), "", message, yAMLSerializer);
        sw.Flush();
        ms.Position = 0;
        StreamReader sr = new(ms);
        string dataToSend = sr.ReadToEnd();

        form.AddField("message", dataToSend);
        form.AddField("message_type", message.GetType().AssemblyQualifiedName);

        if (receiver != null)
            form.AddField("receiver_id", receiver);


        Debug.Log("Sending: " + header + ", " + dataToSend);

        SendRequestWithRetry(BaseUrl + endpoint, form, 3f); 
    }

    private void SendRequestWithRetry(string url, WWWForm form, float retryDelay)
    {
        IEnumerator RetryCoroutine()
        {
            bool success = false;
            while (!success)
            {
                using UnityWebRequest www = UnityWebRequest.Post(url, form);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning("Netzwerkfehler: " + www.error);
                }
                else
                {
                    try
                    {
                        string json = www.downloadHandler.text;
                        var result = JsonUtility.FromJson<ServerResponse>(json);
                        if (result != null && result.success)
                        {
                            success = true;
                            Debug.Log("Nachricht erfolgreich gesendet:");
                        }
                        else
                        {
                            Debug.LogWarning("Serverantwort ohne Erfolg: " + json);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Fehler beim Parsen der Serverantwort: " + e.Message);
                    }
                }

                if (!success)
                {
                    Debug.Log("Wiederhole in " + retryDelay + " Sekunden...");
                    yield return new WaitForSeconds(retryDelay);
                }
            }
        }

        GameNetworkManager.Instance.RunCoroutine(RetryCoroutine());
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
                    Type? type = Type.GetType(msg.MessageType);
                    if (type == null)
                    {
                        Debug.LogError($"Unknown type: {msg.MessageType}");
                        continue;
                    }

                    //object? deserialized = JsonConvert.DeserializeObject(msg.Message, type);

                    MemoryStream ms = new MemoryStream();
                    StreamWriter sw = new StreamWriter(ms);

                    sw.Write(msg.Message);

                    sw.Flush();

                    ms.Position = 0;

                    StreamReader sr = new StreamReader(ms);

                    YAMLDeserializer yAMLDerializer = new YAMLDeserializer(sr);

                    object? deserialized = ReflectionSerializer.DeserializeField(type, "", yAMLDerializer);

                    if (deserialized == null)
                    {
                        Debug.LogError($"Failed to deserialize message: {msg.Message}");
                        continue;
                    }

                    Recieve(new NetworkMessage(msg.Header, deserialized, (PlayerID)int.Parse(msg.Sender)));
                }
            }
            catch (Exception ex)
            { 
                Debug.Log("JSON Deserialization Error: " + ex.Message);
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

[Serializable]
public class ServerResponse
{
    public bool success;
    public string message;
    public string error;
}

[Serializable]
public class MessageData
{
    public string Header;
    public string Message;
    public string Sender;
    public string MessageType;
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