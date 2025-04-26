using System.Collections.Generic;

public class ResourcePledge : ISerializable<ResourcePledge>
{
    public IReadOnlyDictionary<SharedGoalID, IReadOnlyDictionary<Resource, int>> goalPledges { get; private set; } = new Dictionary<SharedGoalID, IReadOnlyDictionary<Resource, int>>();

    public ResourcePledge() { }

    public ResourcePledge(Dictionary<SharedGoalID, IReadOnlyDictionary<Resource, int>> goalPledges)
    {
        this.goalPledges = goalPledges;
    }

    ResourcePledge ISerializable<ResourcePledge>.Deserialize(IDeserializer deserializer)
    {
        goalPledges = deserializer.ReadDict<SharedGoalID, IReadOnlyDictionary<Resource, int>>(
                nameof(goalPledges),
                (string keyName) => deserializer.ReadSerializable<SharedGoalID>(keyName),
                (string valueName) => deserializer.ReadDict<Resource, int>(
                    valueName,
                    (string innerKeyName) => deserializer.ReadEnum<Resource>(innerKeyName),
                    (string innerValueName) => deserializer.ReadInt(innerValueName)
                )
            );
        return this;
    }

    void ISerializable<ResourcePledge>.Serialize(ISerializer serializer)
    {
        serializer.WriteDict(
        nameof(goalPledges),
        goalPledges,
        (string keyName, SharedGoalID key) => serializer.WriteSerializable(keyName, key),
        (string valueName, IReadOnlyDictionary<Resource, int> innerDict) =>
            serializer.WriteDict(
                valueName,
                innerDict,
                (string innerKeyName, Resource resource) => serializer.WriteEnum(innerKeyName, resource),
                (string innerValueName, int amount) => serializer.WriteInt(innerValueName, amount)
            )
    );
    }
}