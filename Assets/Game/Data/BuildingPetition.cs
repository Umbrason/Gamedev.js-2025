using System.Collections.Generic;
using System.Linq;

public class BuildingPetition : ISerializable<BuildingPetition>
{
    public PlayerID PlayerID;
    public HexPosition Position;
    public Building Building;
    public Dictionary<PlayerID, Dictionary<Resource, int>> ResourceSources;

    public bool IsFinanced()
    {
        foreach (var (resource, cost) in Building.ConstructionCosts())
        {
            var provided = ResourceSources.Values.Sum(playerResources => playerResources.GetValueOrDefault(resource));
            if (cost > provided) return false;
        }
        return true;
    }

    public BuildingPetition(PlayerID playerID, HexPosition position, Building building, Dictionary<PlayerID, Dictionary<Resource, int>> resourceSources)
    {
        PlayerID = playerID;
        Position = position;
        Building = building;
        ResourceSources = resourceSources;
    }

    public BuildingPetition(Dictionary<PlayerID, Dictionary<Resource, int>> ResourceSources)
    {
        this.ResourceSources = ResourceSources;
    }

    BuildingPetition ISerializable<BuildingPetition>.Deserialize(IDeserializer deserializer)
    {
        PlayerID = deserializer.ReadEnum<PlayerID>(nameof(PlayerID));
        Position = deserializer.ReadSerializable<HexPosition>(nameof(Position));
        Building = deserializer.ReadEnum<Building>(nameof(Building));
        ResourceSources = deserializer.ReadDict(
                nameof(ResourceSources),
                keyName => deserializer.ReadEnum<PlayerID>(keyName),
                valueName => deserializer.ReadDict(
                    valueName,
                    innerKeyName => deserializer.ReadEnum<Resource>(innerKeyName),
                    innerValueName => deserializer.ReadInt(innerValueName)
                )
            );
        return this;
    }

    void ISerializable<BuildingPetition>.Serialize(ISerializer serializer)
    {
        serializer.WriteEnum(nameof(PlayerID), PlayerID);
        serializer.WriteSerializable(nameof(Position), Position);
        serializer.WriteEnum(nameof(Building), Building);
        serializer.WriteDict(
            nameof(ResourceSources),
            ResourceSources,
            (keyName, key) => serializer.WriteEnum(keyName, key),
            (valueName, innerDict) =>
                serializer.WriteDict(
                    valueName,
                    innerDict,
                    (innerKeyName, resource) => serializer.WriteEnum(innerKeyName, resource),
                    (innerValueName, amount) => serializer.WriteInt(innerValueName, amount)
                )
        );
    }
}