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

    /// <param name="source">resources will be removed from here</param>
    /// <param name="receipt">resources will be added here (and in Collected).</param>
    public void Collect(Dictionary<Resource, int> source, Dictionary<Resource, int> receipt = null)
    {
        foreach (var (resource, required) in Required)
        {
            var missingAmount = required - Collected.GetValueOrDefault(resource);
            var payed = Mathf.Min(source.GetValueOrDefault(resource), missingAmount);
            if (receipt != null)
            {
                if (!receipt.ContainsKey(resource))
                {
                    receipt[resource] = payed;
                }
                else
                    receipt[resource] += payed;
            }
            if (!Collected.ContainsKey(resource)) Collected[resource] = payed;
            else Collected[resource] += payed;
            if (source.ContainsKey(resource)) source[resource] -= payed;
        }
    }

}