using System.Collections.Generic;

public class PlayerData
{
    public string Nickname { get; set; }
    public PlayerRole Role { get; set; }
    public PlayerFactions Faction { get; set; }
    public PlayerIsland Island { get; set; }
    public Dictionary<Resource, int> Resources { get; set; }
    public SecretTask SecretGoal { get; set; }

    public bool HasResources(Dictionary<Resource, int> required)
    {
        foreach (var (resource, amount) in required)
            if (!(Resources.GetValueOrDefault(resource) >= amount)) return false;
        return true;
    }
}