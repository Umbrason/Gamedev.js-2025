using System.Collections.Generic;

public class BuildingPetition
{
    public PlayerID PlayerID;
    public HexPosition Position;
    public Building Building;
    public Dictionary<PlayerID, Dictionary<Resource, int>> ResourceSources;

    public BuildingPetition(PlayerID playerID, HexPosition position, Building building, Dictionary<PlayerID, Dictionary<Resource, int>> resourceSources)
    {
        PlayerID = playerID;
        Position = position;
        Building = building;
        ResourceSources = resourceSources;
    }
}