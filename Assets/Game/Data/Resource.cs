using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Resource
{
    None = 0,

    //Basic
    Dewdrops = 1,
    Leaves = 2,
    Pebbles = 3,
    Mana = 4,
    Wood = 5,
    Fireflies = 6,

    //Refined
    Compost = 7,
    Ink = 8,
    Mushrooms = 9,
    Firebugs = 10,
    Wisps = 11,
    ManaStones = 12,
}

public static class ResourceExtention
{
    public static bool IsBasic(this Resource resource) => resource != Resource.None && resource < Resource.Compost;
    public static bool IsCombined(this Resource resource) => resource >= Resource.Compost;


    public static void Add(this Dictionary<Resource, int> resources, Dictionary<Resource, int> added)
    {
        foreach ((Resource res, int quantity) in added)
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