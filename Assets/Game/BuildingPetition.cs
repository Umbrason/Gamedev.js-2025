using System.Collections.Generic;

public class BuildingPetition
{
    PlayerID PlayerID;
    HexPosition Position;
    Building Building;
    Dictionary<PlayerID, Dictionary<Resource, int>> ResourceSources;
}