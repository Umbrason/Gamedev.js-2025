using System.Collections.Generic;

public class BuildingPetition
{
    public PlayerID PlayerID;
    public HexPosition Position;
    public Building Building;
    public Dictionary<PlayerID, Dictionary<Resource, int>> ResourceSources;
}