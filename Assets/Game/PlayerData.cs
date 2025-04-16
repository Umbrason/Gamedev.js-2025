using System.Collections.Generic;

public class PlayerData
{
    public string Nickname { get; set; }
    public PlayerRole Role { get; set; }
    public PlayerFaction Faction { get; set; }
    public PlayerIsland Island { get; set; }
    public Dictionary<Resource, int> Resources { get; set; }
    public Goal PrivateGoal { get; set; }
}