using System.Collections.Generic;
using System.Linq;

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
    FirebugCradle,  // Wood + Fireflies
    WispNursery,            // Fireflies + Mana
    ManaSolidifier,         // Mana + Earth
}

public static class BuildingExtensions
{
    // TODO: define Resource values
    public static Dictionary<Resource, int> ConstructionCosts(this Building building)
    => building switch
    {
        Building.DewCollector => new() {
            { Resource.Dewdrops, 2 },
            { Resource.Pebbles, 2 },
            { Resource.Fireflies, 2 }
        },
        Building.WoodCollector => new() {
            { Resource.Wood, 2 },
            { Resource.Fireflies, 2 },
            { Resource.Leaves, 2 }
        },
        Building.LeafCollector => new() {
            { Resource.Leaves, 2 },
            { Resource.Dewdrops, 2 },
            { Resource.Mana, 2 }
        },
        Building.EarthCollector => new() {
            { Resource.Pebbles, 2 },
            { Resource.Mana, 2 },
            { Resource.Wood, 2 }
        },
        Building.FireflyCollector => new() {
            { Resource.Fireflies, 2 },
            { Resource.Wood, 2 },
            { Resource.Pebbles, 2 }
        },
        Building.ManaCollector => new() {
            { Resource.Mana, 2 },
            { Resource.Leaves, 2 },
            { Resource.Dewdrops, 2 }
        },
        Building.Composter => new() {
            { Resource.Pebbles, 7 },
            { Resource.Leaves, 7 },
            { Resource.Fireflies, 3 },
            { Resource.Mana, 3 }
        },
        Building.WispNursery => new() {
            { Resource.Fireflies, 7 },
            { Resource.Mana, 7 },
            { Resource.Dewdrops, 3 },
            { Resource.Wood, 3 }
        },
        Building.ManaSolidifier => new() {
            { Resource.Mana, 7 },
            { Resource.Pebbles, 7 },
            { Resource.Leaves, 3 },
            { Resource.Dewdrops, 3 }
        },
        Building.MushroomsFarm => new() {
            { Resource.Wood,7 },
            { Resource.Dewdrops, 7 },
            { Resource.Fireflies, 3 },
            { Resource.Leaves, 3 }
        },
        Building.FirebugCradle => new() {
            { Resource.Fireflies, 7 },
            { Resource.Wood, 7 },
            { Resource.Mana, 3 },
            { Resource.Pebbles, 3 }
        },
        Building.InkGrinder => new() {
            { Resource.Dewdrops, 7 },
            { Resource.Leaves, 7 },
            { Resource.Pebbles, 3 },
            { Resource.Wood, 3 }
        },
        _ => new()
    };

    public static Dictionary<Resource, int> OperationCosts(this Building building)
     => building switch
     {
         Building.InkGrinder => new() { { Resource.Leaves, 1 }, { Resource.Dewdrops, 1 } },
         Building.MushroomsFarm => new() { { Resource.Dewdrops, 1 }, { Resource.Wood, 1 } },
         Building.FirebugCradle => new() { { Resource.Wood, 1 }, { Resource.Fireflies, 1 } },
         Building.WispNursery => new() { { Resource.Fireflies, 1 }, { Resource.Mana, 1 } },
         Building.ManaSolidifier => new() { { Resource.Mana, 1 }, { Resource.Pebbles, 1 } },
         Building.Composter => new() { { Resource.Leaves, 1 }, { Resource.Pebbles, 1 } },
         _ => new()
     };

    public static Resource ResourceYieldType(this Building building) => (Resource)building;


    /// <summary>
    /// Produced .3333 resources per tile. 3 tiles => 1, 6 tiles => 2. partial yield is decided by a coin toss (i.e. 0.3 yield => 30% chance to gain +1)
    /// </summary>
    public static float ExpectedYield(this Building building, PlayerIsland island, HexPosition position)
    {
        if ((int)building > 6) return 1;
        const float yieldPerTile = 1 / 3f;
        var yield = position.GetSurrounding().Count(pos => island.Tiles.GetValueOrDefault(pos) == (Tile)building && island.Buildings.GetValueOrDefault(pos) == Building.None) * yieldPerTile;
        return yield;
    }

    public static HexPosition FindBestPosition(this Building building, PlayerIsland island, out float averageYield)
    {
        averageYield = -10f;
        HexPosition bestPosition = default;

        foreach (HexPosition pos in island.Tiles.Keys)
        {
            if (island.Buildings.GetValueOrDefault(pos) != Building.None) continue;

            float yield = ExpectedYield(building, island, pos);

            foreach (HexPosition adj in pos.GetSurrounding())
            {
                Building other = island.Buildings.GetValueOrDefault(adj);

                if (island.Buildings.GetValueOrDefault(adj) == Building.None) continue;
                if (island.Buildings.GetValueOrDefault(adj) >= Building.Composter) continue;//crafters don't care about occupied tiles

                yield -= 1;//Takes into account the loss of yield of other buildings
            }

            yield *= 0.5f;//average yield is half the maximum yield

            if (yield <= averageYield) continue;

            averageYield = yield;
            bestPosition = pos;
        }

        return bestPosition;
    }
}