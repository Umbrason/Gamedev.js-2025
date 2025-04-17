using System;
using System.Collections.Generic;

public interface INetworkChannel
{
    void SendMessage(string header, object message, PlayerID playerID);
    void BroadcastMessage(string header, object message);
    Dictionary<string, Action<NetworkMessage>> MessageListeners { get; }
    Dictionary<string, Queue<NetworkMessage>> MessageBacklog { get; }
    void StartListening(string header, Action<NetworkMessage> OnRecieved)
    {
        while (MessageBacklog.TryGetValue(header, out var queue) && queue?.Count > 0)
            OnRecieved.Invoke(queue.Dequeue());
        MessageListeners[header] = OnRecieved;
    }
    void StopListening(string header)
    {
        MessageListeners.Remove(header);
    }
}