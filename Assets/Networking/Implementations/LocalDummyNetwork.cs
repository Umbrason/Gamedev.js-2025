using System;
using System.Collections.Generic;

public class LocalDummyNetwork : INetworkChannel
{
    public static readonly Queue<LocalDummyNetwork> availableDummyNetworks = new();
    private static readonly Dictionary<PlayerID, INetworkChannel> NetworkParticipants = new();
    public Dictionary<string, Action<NetworkMessage>> MessageListeners { get; } = new();
    public Dictionary<string, Queue<NetworkMessage>> MessageBacklog { get; } = new();
    public PlayerID PlayerID { get; }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    public static void RegisterClearCallback()
    {
        UnityEditor.EditorApplication.playModeStateChanged += (state) =>
        {
            if (state != UnityEditor.PlayModeStateChange.ExitingEditMode) return;
            NetworkParticipants.Clear();
            availableDummyNetworks.Clear();
            for (int i = 0; i < 6; i++)
                availableDummyNetworks.Enqueue(new LocalDummyNetwork());
        };
    }
#endif

    public LocalDummyNetwork()
    {
        PlayerID = (PlayerID)NetworkParticipants.Count;
        NetworkParticipants.Add(PlayerID, this);
    }

    public void BroadcastMessage(string header, object message)
    {
        foreach (var (addr, networkChannel) in NetworkParticipants)
        {
            if (addr == PlayerID) continue;
            networkChannel.Recieve(new NetworkMessage(header, message, PlayerID));
        }
    }

    public void SendMessage(string header, object message, PlayerID playerID)
    {
        if (NetworkParticipants.TryGetValue(playerID, out var networkChannel))
            networkChannel.Recieve(new NetworkMessage(header, message, PlayerID));
    }
}
