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
        Dictionary<PlayerID, Dictionary<Resource, int>> petition =
            deserializer.ReadDict<PlayerID, Dictionary<Resource, int>>(
                nameof(ResourceSources),
                (string keyName) => deserializer.ReadEnum<PlayerID>(keyName),
                (string valueName) => deserializer.ReadDict<Resource, int>(
                    valueName,
                    (string innerKeyName) => deserializer.ReadEnum<Resource>(innerKeyName),
                    (string innerValueName) => deserializer.ReadInt(innerValueName)
                )
            );

        return new BuildingPetition(petition);
    }

    void ISerializable<BuildingPetition>.Serialize(ISerializer serializer)
    {
        serializer.WriteDict(
            nameof(ResourceSources),
            ResourceSources,
            (string keyName, PlayerID key) => serializer.WriteEnum(keyName, key),
            (string valueName, Dictionary<Resource, int> innerDict) =>
                serializer.WriteDict(
                    valueName,
                    innerDict,
                    (string innerKeyName, Resource resource) => serializer.WriteEnum(innerKeyName, resource),
                    (string innerValueName, int amount) => serializer.WriteInt(innerValueName, amount)
                )
        );
    }
}