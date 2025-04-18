using System.Collections.Generic;

public static class GoalTemplates
{
    public static IReadOnlyList<SharedGoal> BalanceFaction { get; } = new List<SharedGoal>() { new("A", null), new("B", null), new("C", null) };
    public static IReadOnlyList<SharedGoal> SelfishFaction { get; } = new List<SharedGoal>() { new("EA", null), new("EB", null), new("EC", null) };
    public static IReadOnlyList<SecretTask> IndividualSecretTasks { get; } = new List<SecretTask>() { null };
}