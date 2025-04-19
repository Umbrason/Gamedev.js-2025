using System.Collections.Generic;

public static class NetworkUtils
{
    public static bool DistributedRandomDecision(this INetworkChannel networkChannel, PlayerID ClientID, string header, ref Dictionary<PlayerID, float> results)
    {
        if (results != null) return results.Count >= 6;
        var dict = new Dictionary<PlayerID, float>();
        results = dict;
        void ResultCallback(NetworkMessage message)
        {
            //dict[message.sender] = (float)message.content;
            dict[message.sender] = float.Parse(message.content.ToString());
            if (dict.Count >= 6) networkChannel.StopListening(header);
        }
        var value = UnityEngine.Random.value;
        results[ClientID] = value;
        networkChannel.StartListening(header, ResultCallback);
        networkChannel.BroadcastMessage(header, value);
        return false;
    }


    private readonly static Dictionary<INetworkChannel, Dictionary<string, HashSet<PlayerID>>> ActiveSignals = new();
    public static bool WaitForAllPlayersSignal(this INetworkChannel networkChannel, string header, PlayerID ClientID)
    {
        var ActiveSignals = NetworkUtils.ActiveSignals.GetValueOrDefault(networkChannel) ?? (NetworkUtils.ActiveSignals[networkChannel] = new());
        if (ActiveSignals.ContainsKey(header))
        {
            if (ActiveSignals[header].Count < 6)
                return false;
            ActiveSignals.Remove(header);
            return true;
        }
        void SignalCallback(NetworkMessage message)
        {
            ActiveSignals[header].Add(message.sender);
            if (ActiveSignals[header].Count >= 6) networkChannel.StopListening(header);
        }
        ActiveSignals[header] = new() { ClientID };
        networkChannel.StartListening(header, SignalCallback);
        networkChannel.BroadcastMessage(header, null);
        return false;
    }
}