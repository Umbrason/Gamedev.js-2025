public struct SharedGoalID : ISerializable<SharedGoalID>
{
    public PlayerRole TargetRole;
    public int SubgoalIndex;

    public SharedGoalID(PlayerRole targetRole, int subgoalIndex)
    {
        TargetRole = targetRole;
        SubgoalIndex = subgoalIndex;
    }

    public readonly SharedGoal GetGoal(GameInstance game)
    {
        var list = TargetRole switch
        {
            PlayerRole.Balanced => game.BalancedFactionGoals,
            PlayerRole.Selfish => game.SelfishFactionGoals,
            _ => null
        };
        return list[SubgoalIndex];
    }

    SharedGoalID ISerializable<SharedGoalID>.Deserialize(IDeserializer deserializer)
    {
        PlayerRole role = deserializer.ReadEnum<PlayerRole>(nameof(TargetRole));
        int index = deserializer.ReadInt(nameof(SubgoalIndex));
        return new SharedGoalID(role, index);
    }

    void ISerializable<SharedGoalID>.Serialize(ISerializer serializer)
    {
        serializer.WriteEnum(nameof(TargetRole), TargetRole);
        serializer.WriteInt(nameof(SubgoalIndex), SubgoalIndex);
    }
}