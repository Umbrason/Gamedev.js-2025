using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SharedGoal
{
    public SharedGoal(string name, Dictionary<Resource, int> required)
    {
        Name = name;
        Required = required;
        Collected = new();
    }

    public string Name { get; }
    public Dictionary<Resource, int> Collected { get; }
    public Dictionary<Resource, int> Required { get; }
    public float NormalizedProgress => Collected.Values.Sum() / Required.Values.Sum();
    public bool Complete => Collected.Values.Sum() >= Required.Values.Sum();
    public bool Fulfilled
    {
        get
        {
            foreach (var (resource, amount) in Required)
                if (Collected.GetValueOrDefault(resource) < amount) return false;
            return true;
        }
    }

    public void Collect(Dictionary<Resource, int> source)
    {
        foreach (var (resource, required) in Required)
        {
            var missingAmount = required - Collected.GetValueOrDefault(resource);
            var payed = Mathf.Min(source.GetValueOrDefault(resource), missingAmount);
            if (!Collected.ContainsKey(resource)) Collected[resource] = payed;
            else Collected[resource] += payed;
            if (source.ContainsKey(resource)) source[resource] -= payed;
        }
    }

}