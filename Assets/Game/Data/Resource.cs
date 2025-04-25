public enum Resource
{
    None,

    //Basic
    Dewdrops,
    Leaves,
    Pebbles,
    Mana,
    Wood,
    Fireflies,

    //Refined
    Compost,
    Ink,
    Mushrooms,
    Firebugs,
    Wisps,
    ManaStones,
}

public static class ResourceExtention
{
    public static bool IsBasic(this Resource resource) => resource != Resource.None && resource < Resource.Compost;
    public static bool IsCombined(this Resource resource) => resource >= Resource.Compost;
}