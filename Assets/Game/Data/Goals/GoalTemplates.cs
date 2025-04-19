using System.Collections.Generic;

public static class GoalTemplates
{
    public static IReadOnlyList<SharedGoal> BalanceFaction { get; } = new List<SharedGoal>()
    { 
        new("Goal A", new Dictionary<Resource, int>()
        {
            { Resource.A, 20 },
            { Resource.B, 20 },
            { Resource.C, 20 },
            { Resource.D, 20 },
            { Resource.E, 20 },
            { Resource.F, 20 },
        }), 
        new("Goal B", new Dictionary<Resource, int>()
        {
            { Resource.A, 20 },
            { Resource.B, 20 },
            { Resource.C, 20 },
            { Resource.D, 20 },
            { Resource.E, 20 },
            { Resource.F, 20 },
        }),
        new("Goal C", new Dictionary<Resource, int>()
        {
            { Resource.A, 20 },
            { Resource.B, 20 },
            { Resource.C, 20 },
            { Resource.D, 20 },
            { Resource.E, 20 },
            { Resource.F, 20 },
        })
    };

    public static IReadOnlyList<SharedGoal> SelfishFaction { get; } = new List<SharedGoal>()
    {
        new("EA", new Dictionary<Resource, int>()
        {
            { Resource.A, 20 },
            { Resource.B, 20 },
            { Resource.C, 20 },
            { Resource.D, 20 },
            { Resource.E, 20 },
            { Resource.F, 20 },
        }),
        new("EA2", new Dictionary<Resource, int>()
        {
            { Resource.A, 20 },
            { Resource.B, 20 },
            { Resource.C, 20 },
            { Resource.D, 20 },
            { Resource.E, 20 },
            { Resource.F, 20 },
        }),
        new("Be evil", new Dictionary<Resource, int>()
        {
            { Resource.A, 20 },
            { Resource.B, 20 },
            { Resource.C, 20 },
            { Resource.D, 20 },
            { Resource.E, 20 },
            { Resource.F, 20 },
        })
    };
    public static IReadOnlyList<SecretTask> IndividualSecretTasks { get; } = new List<SecretTask>() { null };
}