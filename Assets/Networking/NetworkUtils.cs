using System.Collections.Generic;
using UnityEditor.PackageManager;

public static class NetworkUtils
{
    public static bool DistributedRandomDecision(this INetworkChannel networkChannel, PlayerID ClientID, string header, ref Dictionary<PlayerID, float> results)
    {
        if (results != null) return results.Count >= 6;
        var dict = new Dictionary<PlayerID, float>();
        results = dict;

        void ResultCallback(NetworkMessage message)
        {
            dict[message.sender] = (float)message.content;
            if (dict.Count >= 6) networkChannel.StopListening(header);
        }
        var value = UnityEngine.Random.value;
        results[ClientID] = value;
        networkChannel.StartListening(header, ResultCallback);
        networkChannel.BroadcastMessage(header, value);
        return false;
    }

    private readonly static Dictionary<PlayerID, Dictionary<string, HashSet<PlayerID>>> ActiveSignals = new();
    public static bool WaitForAllPlayersSignal(this INetworkChannel networkChannel, string header, PlayerID ClientID)
    {
        var ActiveSignalsForNetworkChannel = ActiveSignals.GetValueOrDefault(ClientID) ?? (ActiveSignals[ClientID] = new());
        if (ActiveSignalsForNetworkChannel.ContainsKey(header))
        {
            if (ActiveSignalsForNetworkChannel[header].Count < 6) return false;
            ActiveSignalsForNetworkChannel.Remove(header);
            return true;
        }
        void SignalCallback(NetworkMessage message)
        {
            ActiveSignalsForNetworkChannel[header].Add(message.sender);
            if (ActiveSignalsForNetworkChannel[header].Count >= 6) networkChannel.StopListening(header);
        }
        ActiveSignalsForNetworkChannel[header] = new() { ClientID };
        networkChannel.StartListening(header, SignalCallback);
        networkChannel.BroadcastMessage(header, null);
        return false;
    }
}