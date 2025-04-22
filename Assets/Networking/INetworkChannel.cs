using System;
using System.Collections.Generic;

public interface INetworkChannel
{
    #region To Implement
    PlayerID PlayerID { get; }
    string Nickname { get; }
    void SendMessage(string header, object message, PlayerID playerID);
    void BroadcastMessage(string header, object message);
    #endregion


    Dictionary<string, Action<NetworkMessage>> MessageListeners { get; }
    Dictionary<string, Queue<NetworkMessage>> MessageBacklog { get; }
    void Recieve(NetworkMessage message)
    {
        if (MessageListeners.ContainsKey(message.header))
        {
            MessageListeners[message.header].Invoke(message);
            UnityEngine.Debug.Log($"'{PlayerID}' recieved a message from '{message.sender}' with content '{message.content}' on channel '{message.header}'");
            return;
        }
        if (!MessageBacklog.ContainsKey(message.header)) MessageBacklog[message.header] = new();
        MessageBacklog[message.header].Enqueue(message);
    }
    void StartListening(string header, Action<NetworkMessage> OnRecieved)
    {
        MessageListeners[header] = OnRecieved;
        while (MessageBacklog.TryGetValue(header, out var queue) && queue?.Count > 0 && MessageListeners.TryGetValue(header, out var callback))
            callback.Invoke(queue.Dequeue());
        if (MessageBacklog.GetValueOrDefault(header)?.Count == 0) MessageBacklog.Remove(header);
    }
    void StopListening(string header)
    {
        MessageListeners.Remove(header);
    }
    virtual void PullMessages() { }
}