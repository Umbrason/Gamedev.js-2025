using System.Collections.Generic;

public class ResourcePledge : ISerializable<ResourcePledge>
{
    public readonly Dictionary<SharedGoalID, Dictionary<Resource, int>> goalPledges = new();

    public ResourcePledge() { }

    public ResourcePledge(Dictionary<SharedGoalID, Dictionary<Resource, int>> goalPledges)
    {
        this.goalPledges = goalPledges;
    }

    ResourcePledge ISerializable<ResourcePledge>.Deserialize(IDeserializer deserializer)
    {
        Dictionary<SharedGoalID, Dictionary<Resource, int>> pledges =
            deserializer.ReadDict<SharedGoalID, Dictionary<Resource, int>>(
                nameof(goalPledges),
                (string keyName) => deserializer.ReadSerializable<SharedGoalID>(keyName),
                (string valueName) => deserializer.ReadDict<Resource, int>(
                    valueName,
                    (string innerKeyName) => deserializer.ReadEnum<Resource>(innerKeyName),
                    (string innerValueName) => deserializer.ReadInt(innerValueName)
                )
            );

        return new ResourcePledge(pledges);
    }

    void ISerializable<ResourcePledge>.Serialize(ISerializer serializer)
    {
        serializer.WriteDict(
       nameof(goalPledges),
       goalPledges,
       (string keyName, SharedGoalID key) => serializer.WriteSerializable(keyName, key),
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