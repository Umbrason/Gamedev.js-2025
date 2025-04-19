using System;
using System.Collections.Generic;
using System.Linq;

public class PlayerData
{
    public string Nickname { get; set; }
    public PlayerRole Role { get; set; }
    public PlayerFactions Faction { get; set; }
    private PlayerIsland m_Island;
    public PlayerIsland Island { get => m_Island; set { m_Island = value; OnIslandChanged?.Invoke(m_Island); } }
    public Dictionary<Resource, int> Resources { get; set; }
    public SecretTask SecretGoal { get; set; }

    public event Action<PlayerIsland> OnIslandChanged;

    public bool HasResources(Dictionary<Resource, int> required)
    {
        foreach (var (resource, amount) in required)
            if (!(Resources.GetValueOrDefault(resource) >= amount)) return false;
        return true;
    }

    public Dictionary<Building, int> CountBuildings(params Building[] buildings) => buildings.Select((building) => (building, Island.Buildings.Values.Count((b) => b == building))).ToDictionary(pair => pair.building, pair => pair.Item2);
    public int CountBuilding(Building building) => Island.Buildings.Values.Count(v => v == building);
}