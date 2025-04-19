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
    public static Dictionary<Resource, int> ConstructionCosts(this Building building) => new();
    public static Dictionary<Resource, int> OperationCosts(this Building building) => new();
    public static Resource ResourceYieldType(this Building building) => Resource.Dewdrops;
    public static float YieldChanceAt(this Building building, PlayerIsland island, HexPosition position) => 1f;
}