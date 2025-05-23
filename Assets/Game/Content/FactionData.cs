using UnityEngine;
using MapGenerator;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Faction", menuName = "Scriptable Objects/Faction")]
public class FactionData : ScriptableObject
{
    [SerializeField] PlayerFaction faction;
    [SerializeField] TilesBoardGeneratorData mapGenerator;
    [SerializeField] ResourceQuantity[] startingResources;
    [SerializeField] Building[] startingBuildings;


    public PlayerIsland GenerateIsland(int size = 5)
    {
        PlayerIsland island = mapGenerator.Generate(size);

        List<(HexPosition, Building)> Buildings = new();

        foreach(Building building in startingBuildings)
        {
            if(!building.FindBestPosition(island, out HexPosition bestTile, out _)) break;
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

    public PlayerFaction Faction => faction;

    [System.Serializable] class ResourceQuantity
    {
        public Resource resource;
        public int quantity;
    }
}
