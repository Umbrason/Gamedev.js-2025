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
    private readonly Dictionary<Resource, int> m_Resources = new();
    public IReadOnlyDictionary<Resource, int> Resources
    {
        get => m_Resources; set
        {
            foreach (var (resource, amount) in value)
                this[resource] = amount;
        }
    }
    public SecretTask SecretGoal { get; set; }

    public int this[Resource resource]
    {
        get => Resources.GetValueOrDefault(resource);
        set
        {
            if (this[resource] == value) return;
            m_Resources[resource] = value;
            OnResourceChanged?.Invoke(resource, value);
        }
    }

    public event Action<PlayerIsland> OnIslandChanged;
    public event Action<Resource, int> OnResourceChanged;

    public bool HasResources(Dictionary<Resource, int> required)
    {
        foreach (var (resource, amount) in required)
            if (!((Resources?.GetValueOrDefault(resource) ?? 0) >= amount)) return false;
        return true;
    }

    public Dictionary<Building, int> CountBuildings(params Building[] buildings) => buildings.Select((building) => (building, Island.Buildings.Values.Count((b) => b == building))).ToDictionary(pair => pair.building, pair => pair.Item2);
    public int CountBuilding(Building building) => Island.Buildings.Values.Count(v => v == building);
}