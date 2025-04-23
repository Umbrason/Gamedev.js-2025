using UnityEngine;
using MapGenerator;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Faction", menuName = "Scriptable Objects/Faction")]
public class FactionData : ScriptableObject
{
    [SerializeField] PlayerFactions faction;
    [SerializeField] TilesBoardGeneratorData mapGenerator;
    [SerializeField] ResourceQuantity[] startingResources;
    [SerializeField] Building[] startingBuildings;


    public PlayerIsland GenerateIsland(int size = 5)
    {
        PlayerIsland island = mapGenerator.Generate(size);

        List<(HexPosition, Building)> Buildings = new();

        foreach(Building building in startingBuildings)
        {
            HexPosition bestTile = building.FindBestPosition(island, out _);
            Buildings.Add((bestTile, building));
            island = island.WithBuildings(Buildings.ToArray());
        }

        return island;
    }

    public Dictionary<Resource, int> StartingResouces
    {
        get
        {
            Dictionary<Resource, int> Resources = new();

            foreach (ResourceQuantity rq in startingResources)
                Resources[rq.resource] = rq.quantity;

            return Resources;
        }
    }

    public PlayerFactions Faction => faction;

    [System.Serializable] class ResourceQuantity
    {
        public Resource resource;
        public int quantity;
    }
}
