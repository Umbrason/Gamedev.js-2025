using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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


    public static HashSet<Resource> GetAll() => new(){ Resource.Dewdrops, Resource.Leaves, Resource.Pebbles, Resource.Mana, Resource.Wood, Resource.Fireflies, Resource.Compost, Resource.Ink, Resource.Mushrooms, Resource.Firebugs, Resource.Wisps, Resource.ManaStones };

    public static HashSet<Resource> GetAllBasic() => new() { Resource.Dewdrops, Resource.Leaves, Resource.Pebbles, Resource.Mana, Resource.Wood, Resource.Fireflies };

    public static HashSet<Resource> GetAllCombined() => new() { Resource.Compost, Resource.Ink, Resource.Mushrooms, Resource.Firebugs, Resource.Wisps, Resource.ManaStones };

    public static void Add(this Dictionary<Resource, int> resources, Dictionary<Resource, int> added)
    {
        foreach((Resource res, int quantity) in added)
        {
            resources[res] = resources.GetValueOrDefault(res) + added[res];
        }
    }

    /// <summary>
    /// Removes "removed" resources from both dictionaries (don't create negative quantities)
    /// </summary>
    public static void Remove(this Dictionary<Resource, int> resources, Dictionary<Resource, int> removed, out Dictionary<Resource, int> Transfer)
    {
        Transfer = new();
        foreach (Resource res in removed.Keys.ToList())
        {
            Transfer[res] = Mathf.Min(resources.GetValueOrDefault(res), removed[res]);
            resources[res] = resources.GetValueOrDefault(res) - Transfer[res];
            removed[res] -= Transfer[res];
        }
    }
}