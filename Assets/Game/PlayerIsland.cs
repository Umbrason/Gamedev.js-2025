using System.Collections.Generic;

public struct PlayerIsland
{
    IReadOnlyDictionary<HexPosition, Tile> Tiles;
    IReadOnlyDictionary<HexPosition, Building> Buildings;
}