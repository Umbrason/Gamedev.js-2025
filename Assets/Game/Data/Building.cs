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
    MushroomFarm,          // Dewdrops + Wood
    FirebugCradle,  // Wood + Fireflies
    WispNursery,            // Fireflies + Mana
    ManaSolidifier,         // Mana + Earth
}

public static class BuildingExtensions
{
    // TODO: define Resource values
    public static Dictionary<Resource, int> ConstructionCosts(this Building building)
    => new(GameSettings.ConstructionCosts.GetValueOrDefault(building));

    public static Dictionary<Resource, int> OperationCosts(this Building building)
     => new(GameSettings.OperationCosts.GetValueOrDefault(building));

    public static Resource ResourceYieldType(this Building building) => (Resource)building;

    const float YIELD_PER_TILE = 1 / 3f;

    /// <summary>
    /// Produced .3333 resources per tile. 3 tiles => 1, 6 tiles => 2. partial yield is decided by a coin toss (i.e. 0.3 yield => 30% chance to gain +1)
    /// </summary>
    public static float ExpectedYield(this Building building, PlayerIsland island, HexPosition position)
    {
        /* 
            Note: the AI use this method. It should returns the average amount produced. 
            If this changes, please make also a method that estimates the average and make the AI call it.
        */

        if ((int)building > 6) return 1;
        var yield = position.GetSurrounding().Count(pos => island.Tiles.GetValueOrDefault(pos) == (Tile)building && island.Buildings.GetValueOrDefault(pos) == Building.None) * YIELD_PER_TILE;
        return yield;
    }

    public static bool FindBestPosition(this Building building, PlayerIsland island, out HexPosition bestPosition, out float averageYield)
    {
        averageYield = -1000f;
        bestPosition = default;

        foreach (HexPosition pos in island.Tiles.Keys)
        {
            if (island.Buildings.GetValueOrDefault(pos) != Building.None) continue;

            float yield = ExpectedYield(building, island, pos);

            foreach (HexPosition adj in pos.GetSurrounding())
            {
                Building other = island.Buildings.GetValueOrDefault(adj);

                if (island.Buildings.GetValueOrDefault(adj) == Building.None) continue;
                if (island.Buildings.GetValueOrDefault(adj) >= Building.Composter) continue;//crafters don't care about occupied tiles

                yield -= YIELD_PER_TILE;//Takes into account the loss of yield of other buildings
            }

            if (yield <= averageYield) continue;

            averageYield = yield;
            bestPosition = pos;
        }

        return averageYield > -999f;//hex found
    }

    public static string Name(this Building building)
    {
        return System.Text.RegularExpressions.Regex.Replace(building.ToString(), @"([a-z])([A-Z])", "$1~$2").Replace("~", " ");
    }

    public static string Description(this Building building)
    {
        if ((int)building > 6)
        {
            return $"Converts 1 {string.Join(" and ", building.OperationCosts().Keys.Select(r => r.ToString()))} to {building.ResourceYieldType()}";
        }
        return $"Collects {building.ResourceYieldType()} with a chance of 1/3 per free neighbouring {building.ResourceYieldType()} tile";
    }
}