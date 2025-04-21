using System.Collections.Generic;
using System.Linq;

public class BuildingPetition
{
    public PlayerID PlayerID;
    public HexPosition Position;
    public Building Building;
    public Dictionary<PlayerID, Dictionary<Resource, int>> ResourceSources;

    public bool IsFinanced()
    {
        foreach (var (resource, cost) in Building.ConstructionCosts())
        {
            var provided = ResourceSources.Values.Sum(playerResources => playerResources.GetValueOrDefault(resource));
            if (cost > provided) return false;
        }
        return true;
    }

    public BuildingPetition(PlayerID playerID, HexPosition position, Building building, Dictionary<PlayerID, Dictionary<Resource, int>> resourceSources)
    {
        PlayerID = playerID;
        Position = position;
        Building = building;
        ResourceSources = resourceSources;
    }
}