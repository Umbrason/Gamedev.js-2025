using System.Collections.Generic;
using UnityEngine;

public enum Building
{
    None,
    // Basic Collectors
    DewCollector,
    LeafCollector,
    EarthCollector,
    ManaCollector,
    WoodCollector,
    FireflyCollector,

    // Resource Combiners
    Composter,              // Earth + Leaves
    InkGrinder,             // Leaves + Dewdrops
    MushroomsFarm,          // Dewdrops + Wood
    LanternWeavingStation,  // Wood + Fireflies
    WispNursery,            // Fireflies + Mana
    ManaSolidifier,         // Mana + Earth
}

public static class BuildingExtensions
{
    public static Dictionary<Resource, int> ConstructionCosts(this Building building)
    {
        // TODO: define Resource values
        return building switch
        {
            Building.DewCollector => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.LeafCollector => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.EarthCollector => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.ManaCollector => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.WoodCollector => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.FireflyCollector => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.Composter => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.InkGrinder => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.MushroomsFarm => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.LanternWeavingStation => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.WispNursery => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            Building.ManaSolidifier => new Dictionary<Resource, int>()
                { { Resource.Wood, 1 }, { Resource.Wood, 1 } },
            _ => new()
        };
    }
    public static Dictionary<Resource, int> OperationCosts(this Building building)
    {
        Dictionary<Resource, int> result = new();
        switch (building)
        {
            case Building.Composter:
                result.Add(Resource.Leaves, 1);
                result.Add(Resource.Earth, 1);
                break;
            case Building.InkGrinder:
                result.Add(Resource.Leaves, 1);
                result.Add(Resource.Dewdrops, 1);
                break;
            case Building.MushroomsFarm:
                result.Add(Resource.Dewdrops, 1);
                result.Add(Resource.Wood, 1);
                break;
            case Building.LanternWeavingStation:
                result.Add(Resource.Wood, 1);
                result.Add(Resource.Fireflies, 1);
                break;
            case Building.WispNursery:
                result.Add(Resource.Fireflies, 1);
                result.Add(Resource.Mana, 1);
                break;
            case Building.ManaSolidifier:
                result.Add(Resource.Mana, 1);
                result.Add(Resource.Earth, 1);
                break;
        }
        return result;
    }

    public static Resource ResourceYieldType(this Building building)
    {
        return building switch
        {
            Building.None => Resource.None,
            Building.DewCollector => Resource.Dewdrops,
            Building.LeafCollector => Resource.Leaves,
            Building.EarthCollector => Resource.Earth,
            Building.ManaCollector => Resource.Mana,
            Building.WoodCollector => Resource.Wood,
            Building.FireflyCollector => Resource.Fireflies,
            Building.Composter => Resource.Compost,
            Building.InkGrinder => Resource.Ink,
            Building.MushroomsFarm => Resource.Mushrooms,
            Building.LanternWeavingStation => Resource.FireflyLanterns,
            Building.WispNursery => Resource.Wisps,
            Building.ManaSolidifier => Resource.ManaStones,
            _ => Resource.None
        };
    }

    private const float percentagePerTile = 0.15f;
    public static float YieldChanceAt(this Building building, PlayerIsland island, HexPosition position)
    {
        if ((int)building > 6) return 80f;

        float percentage = 0f;
        foreach (HexPosition pos in position.GetSurrounding())
            if (island.Tiles[pos] == (Tile)building) percentage += percentagePerTile;
        return percentage;
    }
}