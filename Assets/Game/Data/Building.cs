using System.Collections.Generic;

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
    // TODO: define Resource values
    public static Dictionary<Resource, int> ConstructionCosts(this Building building)
    => building switch
    {
        Building.DewCollector => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.LeafCollector => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.EarthCollector => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.ManaCollector => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.WoodCollector => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.FireflyCollector => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.Composter => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.InkGrinder => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.MushroomsFarm => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.LanternWeavingStation => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.WispNursery => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        Building.ManaSolidifier => new() { { Resource.Wood, 1 }, { Resource.Leaves, 1 } },
        _ => new()
    };

    public static Dictionary<Resource, int> OperationCosts(this Building building)
     => building switch
     {
         Building.InkGrinder => new() { { Resource.Leaves, 1 }, { Resource.Dewdrops, 1 } },
         Building.MushroomsFarm => new() { { Resource.Dewdrops, 1 }, { Resource.Wood, 1 } },
         Building.LanternWeavingStation => new() { { Resource.Wood, 1 }, { Resource.Fireflies, 1 } },
         Building.WispNursery => new() { { Resource.Fireflies, 1 }, { Resource.Mana, 1 } },
         Building.ManaSolidifier => new() { { Resource.Mana, 1 }, { Resource.Earth, 1 } },
         Building.Composter => new() { { Resource.Leaves, 1 }, { Resource.Earth, 1 } },
         _ => new()
     };

    public static Resource ResourceYieldType(this Building building) => (Resource)building;

    private const float percentagePerTile = 0.15f;
    public static float YieldChanceAt(this Building building, PlayerIsland island, HexPosition position)
    {
        if ((int)building > 6) return .8f; //yield chance for
        float percentage = 0f;
        foreach (HexPosition pos in position.GetSurrounding())
            if (island.Tiles[pos] == (Tile)building) percentage += percentagePerTile;
        return percentage;
    }
}