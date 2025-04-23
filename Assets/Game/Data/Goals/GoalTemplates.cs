using System;
using System.Collections.Generic;
using System.Linq;

public static class GoalTemplates
{
    /*
    public static IReadOnlyList<SharedGoal> BalanceFaction { get; } = new List<SharedGoal>()
    {
        new("Restore balance", new Dictionary<Resource, int>()
        {
            { Resource.Compost, 20 },
            { Resource.Ink, 20 },
            { Resource.Mushrooms, 20 },
            { Resource.FireflyLanterns, 20 },
            { Resource.Wisps, 20 },
            { Resource.ManaStones, 20 }
        })
    };

    public static IReadOnlyList<SharedGoal> SelfishFaction => new List<SharedGoal>()
    {
        new("Seed chaos", new Dictionary<Resource, int>()
        {
            { Resource.Compost, 10 },
            { Resource.Ink, 10 },
            { Resource.Mushrooms, 10 },
            { Resource.Dewdrops, 10 },
            { Resource.Leaves, 10 },
            { Resource.FireflyLanterns, 10 }
        }),
        new("Seed chaos", new Dictionary<Resource, int>()
        {
            { Resource.FireflyLanterns, 10 },
            { Resource.Wisps, 10 },
            { Resource.ManaStones, 10 },
            { Resource.Wood, 10 },
            { Resource.Earth, 10 },
            { Resource.Mana, 10 }
        }),
        new("Seed chaos", new Dictionary<Resource, int>()
        {
            { Resource.Compost, 10 },
            { Resource.Wisps, 10 },
            { Resource.Mushrooms, 10 },
            { Resource.Wood, 10 },
            { Resource.Leaves, 10 },
            { Resource.Mana, 10 }
        }),
        new("Seed chaos", new Dictionary<Resource, int>()
        {
            { Resource.FireflyLanterns, 10 },
            { Resource.Wisps, 10 },
            { Resource.ManaStones, 10 },
            { Resource.Earth, 10 },
            { Resource.Dewdrops, 10 },
            { Resource.Mana, 10 }
        })
    };
    */
    public static IReadOnlyList<SecretTask> IndividualSecretTasks { get; } = ((Building[])Enum.GetValues(typeof(Building))).Select(building => new BuildingAmount($"Have at least 3 {building}'s.", new() { { building, 5 } })).ToList();
}