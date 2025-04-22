using System;
using System.Collections.Generic;

public class LocalDummyNetwork : INetworkChannel
{
    private static readonly Dictionary<PlayerID, Dictionary<string, Queue<NetworkMessage>>> MessageBacklogs = new();
    public static Dictionary<PlayerID, Action<NetworkMessage>> OnMessageRecieved = new();
    public Dictionary<string, Action<NetworkMessage>> MessageListeners { get; } = new();
    Dictionary<string, Queue<NetworkMessage>> INetworkChannel.MessageBacklog => MessageBacklogs.TryGetValue(PlayerID, out var backlog) ? backlog : MessageBacklogs[PlayerID] = new();
    public PlayerID PlayerID { get; }
    public string Nickname { get; }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    public static void RegisterClearCallback()
    {
        UnityEditor.EditorApplication.playModeStateChanged += (state) =>
        {
            if (state != UnityEditor.PlayModeStateChange.ExitingEditMode) return;
            MessageBacklogs.Clear();
            OnMessageRecieved.Clear();
        };
    }
#endif

    public LocalDummyNetwork(PlayerID playerID, string nickname)
    {
        PlayerID = playerID;
        Nickname = nickname;
        OnMessageRecieved[playerID] = ((INetworkChannel)this).Recieve;
    }

    public void BroadcastMessage(string header, object message)
    {
        for (PlayerID id = 0; (int)id < 6; id++)
        {
            if (id == PlayerID) continue;
            SendMessage(header, message, id);
        }
    }

    public void SendMessage(string header, object message, PlayerID recipientID)
    {
        if (!MessageBacklogs.TryGetValue(recipientID, out var queues))
            MessageBacklogs[recipientID] = queues = new();
        if (!queues.TryGetValue(header, out var queue))
            queues[header] = queue = new();
        var netmessage = new NetworkMessage(header, message, PlayerID);
        if (OnMessageRecieved.ContainsKey(recipientID)) OnMessageRecieved[recipientID].Invoke(netmessage);
        else queue.Enqueue(netmessage);
    }
}