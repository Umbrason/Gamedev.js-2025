using System.Collections.Generic;

public enum Building
{
    None,
}

public static class BuildingExtensions
{
    public static Dictionary<Resource, int> ConstructionCosts(this Building building) => default;
    public static Dictionary<Resource, int> OperationCosts(this Building building) => default;
    public static Resource ResourceYieldType(this Building building) => default;
    public static float YieldChanceAt(this Building building, PlayerIsland island, HexPosition position) => default;
}