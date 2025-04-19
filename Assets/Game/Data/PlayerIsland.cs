using System;
using System.Collections.Generic;

public readonly struct PlayerIsland
{
    public readonly IReadOnlyDictionary<HexPosition, Tile> Tiles;
    public readonly IReadOnlyDictionary<HexPosition, Building> Buildings;

    public PlayerIsland(IReadOnlyDictionary<HexPosition, Tile> tiles, IReadOnlyDictionary<HexPosition, Building> buildings)
    {
        Tiles = tiles;
        Buildings = buildings;
    }

    public static PlayerIsland Empty => new(null, null);

    public static bool operator ==(PlayerIsland A, PlayerIsland B)
    {
        if (((A.Tiles?.Count ?? -1) != (B.Tiles?.Count ?? -1)) || ((A.Buildings?.Count ?? -1) != (B.Buildings?.Count ?? -1))) return false;
        if (A.Buildings != null) foreach (var (pos, building) in A.Buildings) if (!B.Buildings.ContainsKey(pos) || B.Buildings[pos] != building) return false;
        if (A.Tiles != null) foreach (var (pos, tile) in A.Tiles) if (!B.Tiles.ContainsKey(pos) || B.Tiles[pos] != tile) return false;
        return true;
    }
    public static bool operator !=(PlayerIsland A, PlayerIsland B) => !(A == B);

    public override bool Equals(object obj)
    {
        if (obj is PlayerIsland island) return island == this;
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Tiles, Buildings);
    }

    private PlayerIsland Copy()
    {
        var TilesCopy = new Dictionary<HexPosition, Tile>();
        foreach (var (k, v) in Tiles) TilesCopy.Add(k, v);
        var BuildingsCopy = new Dictionary<HexPosition, Building>();
        foreach (var (k, v) in Buildings) BuildingsCopy.Add(k, v);
        return new(TilesCopy, BuildingsCopy);
    }

    public PlayerIsland WithTiles(params (HexPosition, Tile)[] tiles)
    {
        var cpy = Copy();
        foreach (var (pos, tile) in tiles)
        {
            if (tile == Tile.None) ((Dictionary<HexPosition, Tile>)cpy.Tiles).Remove(pos);
            else ((Dictionary<HexPosition, Tile>)cpy.Tiles)[pos] = tile;
        }
        return cpy;
    }

    public PlayerIsland WithBuildings(params (HexPosition, Building)[] buildings)
    {
        var cpy = Copy();
        foreach (var (pos, building) in buildings)
        {
            if (building == Building.None) ((Dictionary<HexPosition, Building>)cpy.Buildings).Remove(pos);
            else ((Dictionary<HexPosition, Building>)cpy.Buildings)[pos] = building;
        }
        return cpy;
    }
}