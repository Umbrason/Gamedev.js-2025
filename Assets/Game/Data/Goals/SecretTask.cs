using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public abstract class SecretTask
{
    public abstract string Description { get; protected set; }
    public abstract bool Evaluate(PlayerData data);
    public override string ToString() => Description;
}

public class BuildingAmount : SecretTask
{
    public override string Description { get => Description; protected set => Description = value; }

    private Dictionary<Building, int> requiredBuildings;

    public BuildingAmount(string description, Dictionary<Building, int> buildings)
    {
        Description = description;
        requiredBuildings = buildings;
    }


    public override bool Evaluate(PlayerData data)
    {
        foreach (var (building, amount) in requiredBuildings)
            if (data.CountBuilding(building) < amount) return false;
        return true;
    }
}


public class RequestsFulfilled : SecretTask
{
    public override string Description { get => Description; protected set => Description = value; }
    private int amount;


    public RequestsFulfilled(int _amount)
    {
        amount = _amount;
        Description = $"Have {amount} of your requests accepted";
    }

    public override bool Evaluate(PlayerData data)
    {
        return false;
    }
}