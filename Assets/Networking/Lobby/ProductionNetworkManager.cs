//#define NetworkDebug

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ProductionNetwork : INetworkChannel
{
    public string RoomCode { get; private set; }
    public int ServerPlayerID { get; private set; }

    public Dictionary<string, Action<NetworkMessage>> MessageListeners { get; } = new();
    public Dictionary<string, Queue<NetworkMessage>> MessageBacklog { get; } = new();

    public PlayerID PlayerID { get; }
    public string Nickname { get; }

    public const string BaseUrl = "https://jamapi.heggie.dev/";

    public ProductionNetwork(PlayerID playerID, string roomCode, int serverPlayerID, string nickname)
    {
        RoomCode = roomCode;
        ServerPlayerID = serverPlayerID;
        PlayerID = playerID;
        Nickname = nickname;
    }

    public void SendMessage(string header, object message, PlayerID receiver)
    {
        if(receiver == PlayerID)
        {
#if NetworkDebug
            Debug.Log("Receiver cannot be same as sender");
#endif
            return;
        }
        SendToServer("sendMessage_v2.php", header, message, ((int)receiver).ToString());
    }

    public void BroadcastMessage(string header, object message)
    {
        SendToServer("broadcastMessage_v2.php", header, message, null);
    }

    private void SendToServer(string endpoint, string header, object message, string receiver)
    {
        WWWForm form = new();
        form.AddField("room_code", RoomCode);
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
            form.AddField("receiver_game_id", receiver);

#if NetworkDebug
        Debug.Log($"Sending: {header}, {dataToSend}, as {PlayerID}");
#endif

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
#if NetworkDebug
                    Debug.LogWarning("Netzwerkfehler: " + www.error);
#endif
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
#if NetworkDebug
                            Debug.Log("Message sended succesfully:");
#endif
                        }
                        else
                        {
#if NetworkDebug
                            Debug.LogWarning("Server response error: " + json);
#endif
                        }
                    }
                    catch (Exception e)
                    {
#if NetworkDebug
                        Debug.LogWarning("Error parsing serverreply: " + e.Message);
#endif
                    }
                }

                if (!success)
                {
#if NetworkDebug
                    Debug.Log("Retry in " + retryDelay + " seconds...");
#endif
                    yield return new WaitForSeconds(retryDelay);
                }
            }
        }

        GameNetworkManager.Instance.RunCoroutine(RetryCoroutine());
    }

    public void PullMessages()
    {
        UnityWebRequest www = UnityWebRequest.Get($"{BaseUrl}getMessages_v2.php?room_code={RoomCode}&player_game_id={(int)PlayerID}");
        www.SendWebRequest().completed += _ =>
        {
            if (www.result != UnityWebRequest.Result.Success)
            {
#if NetworkDebug
                Debug.LogError("Message Polling Error: " + www.error);
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

                    //object? deserialized = JsonConvert.DeserializeObject(msg.Message, type);

                    MemoryStream ms = new MemoryStream();
                    StreamWriter sw = new StreamWriter(ms);

                    sw.Write(msg.Message);

                    sw.Flush();

                    ms.Position = 0;

                    StreamReader sr = new StreamReader(ms);

                    YAMLDeserializer yAMLDerializer = new YAMLDeserializer(sr);

                    object deserialized = ReflectionSerializer.DeserializeField(type, "", yAMLDerializer);

                    if (deserialized == null)
                    {
#if NetworkDebug
                        Debug.LogError($"Failed to deserialize message: {msg.Message}");
#endif
                        continue;
                    }

                    Recieve(new NetworkMessage(msg.Header, deserialized, (PlayerID)int.Parse(msg.Sender)));
                }
            }
            catch (Exception ex)
            {
#if NetworkDebug
                Debug.Log("JSON Deserialization Error: " + ex.Message);
#endif
            }
        };
    }

    public void Recieve(NetworkMessage message)
    {
#if NetworkDebug
        Debug.Log($"Received Message {message.header}, {message.content}, From: {message.sender}");
#endif

        if (MessageListeners.ContainsKey(message.header))
        {
            MessageListeners[message.header].Invoke(message);
        }
        else
        {
            if (!MessageBacklog.ContainsKey(message.header))
                MessageBacklog[message.header] = new Queue<NetworkMessage>();

            MessageBacklog[message.header].Enqueue(message);
        }
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

    public string Receiver;
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