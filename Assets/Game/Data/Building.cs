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
    => GameSettings.ConstructionCosts.GetValueOrDefault(building);

    public static Dictionary<Resource, int> OperationCosts(this Building building)
     => GameSettings.OperationCosts.GetValueOrDefault(building);

    public static Resource ResourceYieldType(this Building building) => (Resource)building;

    
    /// <summary>
    /// Each turn a building produces between 0 and MaxYield resources
    /// </summary>
    public static int MaxYieldAt(this Building building, PlayerIsland island, HexPosition position)
    {
        if ((int)building > 6) return 2;//averages to 1 per turn
        int yield = 0;
        foreach (HexPosition pos in position.GetSurrounding())
            if (island.Tiles.GetValueOrDefault(pos) == (Tile)building && island.Buildings.GetValueOrDefault(pos) == Building.None) yield += 1;
        return yield;
    }

   public static HexPosition FindBestPosition(this Building building, PlayerIsland island, out float averageYield)
    {
        averageYield = -10f;
        HexPosition bestPosition = default;

        foreach (HexPosition pos in island.Tiles.Keys)
        {
            if (island.Buildings.GetValueOrDefault(pos) != Building.None) continue;

            float yield = MaxYieldAt(building, island, pos);

            foreach(HexPosition adj in pos.GetSurrounding())
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