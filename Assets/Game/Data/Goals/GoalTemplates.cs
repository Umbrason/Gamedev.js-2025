using System.Collections.Generic;

public static class GoalTemplates
{
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
        }),
        
        /*
        new("Fertilize", new Dictionary<Resource, int>()
        {
            { Resource.Compost, 30 }
        }), 
        new("Write stories", new Dictionary<Resource, int>()
        {
            { Resource.Ink, 30 }
        }),
        new("Symbiosis", new Dictionary<Resource, int>()
        {
            { Resource.Mushrooms, 30 }
        }),
        new("Illuminate", new Dictionary<Resource, int>()
        {
            { Resource.FireflyLanterns, 30 }
        }),
        new("Commune", new Dictionary<Resource, int>()
        {
            { Resource.Wisps, 30 }
        }),
        new("Attune", new Dictionary<Resource, int>()
        {
            { Resource.ManaStones, 30 }
        })*/
    };

    public static IReadOnlyList<SharedGoal> SelfishFaction { get; } = new List<SharedGoal>()
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
    public static IReadOnlyList<SecretTask> IndividualSecretTasks { get; } = new List<SecretTask>() { null };
}