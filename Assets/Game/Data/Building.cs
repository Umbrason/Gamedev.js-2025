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
    => GameSettings.ConstructionCosts.GetValueOrDefault(building);

    public static Dictionary<Resource, int> OperationCosts(this Building building)
     => GameSettings.OperationCosts.GetValueOrDefault(building);

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