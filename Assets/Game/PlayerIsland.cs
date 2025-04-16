using System.Collections.Generic;

public struct PlayerIsland
{
    public IReadOnlyDictionary<HexPosition, Tile> Tiles;
    public IReadOnlyDictionary<HexPosition, Building> Buildings;

    public PlayerIsland(IReadOnlyDictionary<HexPosition, Tile> tiles) : this()
    {
        Tiles = tiles;
        Buildings = new Dictionary<HexPosition, Building>();
    }

    public PlayerIsland(IReadOnlyDictionary<HexPosition, Tile> tiles, IReadOnlyDictionary<HexPosition, Building> buildings)
    {
        Tiles = tiles;
        Buildings = buildings;
    }
}