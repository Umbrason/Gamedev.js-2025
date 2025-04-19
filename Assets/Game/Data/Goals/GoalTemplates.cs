using System.Collections.Generic;

public static class GoalTemplates
{
    public static IReadOnlyList<SharedGoal> BalanceFaction { get; } = new List<SharedGoal>()
    { 
        new("Goal A", new Dictionary<Resource, int>()
        {
            { Resource.Dewdrops, 20 },
            { Resource.Leaves, 20 },
            { Resource.Earth, 20 },
            { Resource.Mana, 20 },
            { Resource.Wood, 20 },
            { Resource.Fireflies, 20 },
        }), 
        new("Goal B", new Dictionary<Resource, int>()
        {
            { Resource.Dewdrops, 20 },
            { Resource.Leaves, 20 },
            { Resource.Earth, 20 },
            { Resource.Mana, 20 },
            { Resource.Wood, 20 },
            { Resource.Fireflies, 20 },
        }),
        new("Goal C", new Dictionary<Resource, int>()
        {
            { Resource.Dewdrops, 20 },
            { Resource.Leaves, 20 },
            { Resource.Earth, 20 },
            { Resource.Mana, 20 },
            { Resource.Wood, 20 },
            { Resource.Fireflies, 20 },
        })
    };

    public static IReadOnlyList<SharedGoal> SelfishFaction { get; } = new List<SharedGoal>()
    {
        new("EA", new Dictionary<Resource, int>()
        {
            { Resource.Dewdrops, 20 },
            { Resource.Leaves, 20 },
            { Resource.Earth, 20 },
            { Resource.Mana, 20 },
            { Resource.Wood, 20 },
            { Resource.Fireflies, 20 },
        }),
        new("EA2", new Dictionary<Resource, int>()
        {
            { Resource.Dewdrops, 20 },
            { Resource.Leaves, 20 },
            { Resource.Earth, 20 },
            { Resource.Mana, 20 },
            { Resource.Wood, 20 },
            { Resource.Fireflies, 20 },
        }),
        new("Be evil", new Dictionary<Resource, int>()
        {
            { Resource.Dewdrops, 20 },
            { Resource.Leaves, 20 },
            { Resource.Earth, 20 },
            { Resource.Mana, 20 },
            { Resource.Wood, 20 },
            { Resource.Fireflies, 20 },
        })
    };
    public static IReadOnlyList<SecretTask> IndividualSecretTasks { get; } = new List<SecretTask>() { null };
}