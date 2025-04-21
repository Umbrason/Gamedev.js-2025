public struct SharedGoalID
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
}
