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
        Building.DewCollector => new() { 
            { Resource.Dewdrops, 2 }, 
            { Resource.Earth, 2 }, 
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
            { Resource.Earth, 2 },
            { Resource.Mana, 2 },
            { Resource.Wood, 2 }
        },
        Building.FireflyCollector => new() {
            { Resource.Fireflies, 2 },
            { Resource.Wood, 2 },
            { Resource.Earth, 2 }
        },
        Building.ManaCollector => new() {
            { Resource.Mana, 2 },
            { Resource.Leaves, 2 },
            { Resource.Dewdrops, 2 }
        },
        Building.Composter => new() {
            { Resource.Earth, 7 },
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
            { Resource.Earth, 7 },         
            { Resource.Leaves, 3 },
            { Resource.Dewdrops, 3 }
        },        
        Building.MushroomsFarm => new() {
            { Resource.Wood,7 },
            { Resource.Dewdrops, 7 },         
            { Resource.Fireflies, 3 },
            { Resource.Leaves, 3 }
        },        
        Building.LanternWeavingStation => new() {
            { Resource.Fireflies, 7 },
            { Resource.Wood, 7 },         
            { Resource.Mana, 3 },
            { Resource.Earth, 3 }
        },        
        Building.InkGrinder => new() {
            { Resource.Dewdrops, 7 },
            { Resource.Leaves, 7 },         
            { Resource.Earth, 3 },
            { Resource.Wood, 3 }
        },
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

<<<<<<< Updated upstream
    
    /// <summary>
    /// Each turn a building produces between 0 and MaxYield resources
    /// </summary>
    public static int MaxYieldAt(this Building building, PlayerIsland island, HexPosition position)
    {
        if ((int)building > 6) return 2;//averages to 1 per turn
        int yield = 0;
=======
    private const float percentagePerTile = 0.1f;
    public static float YieldChanceAt(this Building building, PlayerIsland island, HexPosition position)
    {
        if ((int)building > 6) return .8f; //yield chance for combiners (always 80% for now)
        float percentage = 0.15f;
>>>>>>> Stashed changes
        foreach (HexPosition pos in position.GetSurrounding())
            if (island.Tiles.GetValueOrDefault(pos) == (Tile)building && island.Buildings.GetValueOrDefault(pos) == Building.None) yield += 1;
        return yield;
    }

<<<<<<< Updated upstream
   public static HexPosition FindBestPosition(this Building building, PlayerIsland island, out float averageYield)
=======
    public static HexPosition FindBestPosition(this Building building, PlayerIsland island, out float yieldChance)
>>>>>>> Stashed changes
    {
        averageYield = -10f;
        HexPosition bestPosition = default;

        foreach (HexPosition pos in island.Tiles.Keys)
        {
            if (island.Buildings.GetValueOrDefault(pos) != Building.None) continue;

            float yield = MaxYieldAt(building, island, pos);

            // Takes into account the loss of yield of other buildings, excluding Combiners cause they aren't affected by adj tiles for now
            foreach (HexPosition adj in pos.GetSurrounding().Where(x => island.Buildings.GetValueOrDefault(x) != Building.None && island.Buildings.GetValueOrDefault(x) < (Building)7))
            {
<<<<<<< Updated upstream
                Building other = island.Buildings.GetValueOrDefault(adj);

                if (island.Buildings.GetValueOrDefault(adj) == Building.None) continue;
                if (island.Buildings.GetValueOrDefault(adj) >= Building.Composter) continue;//crafters don't care about occupied tiles

                yield -= 1;//Takes into account the loss of yield of other buildings
=======
                yield -= percentagePerTile;
>>>>>>> Stashed changes
            }

            yield *= 0.5f;//average yield is half the maximum yield

            if (yield <= averageYield) continue;

            averageYield = yield;
            bestPosition = pos;
        }

        return bestPosition;
    }
}