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
            { Resource.Firebugs, 20 },
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
            { Resource.Firebugs, 10 }
        }),
        new("Seed chaos", new Dictionary<Resource, int>()
        {
            { Resource.Firebugs, 10 },
            { Resource.Wisps, 10 },
            { Resource.ManaStones, 10 },
            { Resource.Wood, 10 },
            { Resource.Pebbles, 10 },
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
            { Resource.Firebugs, 10 },
            { Resource.Wisps, 10 },
            { Resource.ManaStones, 10 },
            { Resource.Pebbles, 10 },
            { Resource.Dewdrops, 10 },
            { Resource.Mana, 10 }
        })
    };
    */

    public static IReadOnlyList<SecretTask> IndividualSecretTasks { get; } =
        ((Building[])Enum.GetValues(typeof(Building)))
        .Where(building => building != Building.None)
        .Select(building => {
            int n = building >= Building.Composter ? 1 : 3;
            string s = n >= 2 ? "s" : "";
            return new BuildingAmount($"Have at least {n} {building}{s}.\\n\\nComplete this task to double all building production.\"", new() { { building, n } }); 
        })
        .ToList();
}