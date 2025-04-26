using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Scriptable Objects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField] FactionData[] factions;
    [SerializeField] BuildingData[] buildings;
    [SerializeField] ResourcesData[] balancedGoals;
    [SerializeField] ResourcesData[] selfishGoals;

    public static ResourcesData[] staticbalancedGoals;
    public static ResourcesData[] staticselfishGoals;

    public static IReadOnlyList<SharedGoal> BalanceGoals => GetSharedGoalList(staticbalancedGoals); //fuck performance we need this done NOW
    public static IReadOnlyList<SharedGoal> SelfishGoals => GetSharedGoalList(staticselfishGoals);
    public static Dictionary<Building, Dictionary<Resource, int>> ConstructionCosts { get; private set; }
    public static Dictionary<Building, Dictionary<Resource, int>> OperationCosts { get; private set; }
    public static FactionData[] staticFactions;


    [RuntimeInitializeOnLoadMethod]
    public static void RuntimeInitializeOnLoad()
    {
        var settings = Resources.Load<GameSettings>("Game Settings");
        settings.Use();
    }

    public void Use()
    {
        GameSettings.staticbalancedGoals = balancedGoals;
        GameSettings.staticselfishGoals = selfishGoals;
        GameSettings.staticFactions = factions;
        ConstructionCosts = new();
        OperationCosts = new();
        foreach (BuildingData building in buildings)
        {
            ConstructionCosts[building.Building] = building.Cost.Items;
            OperationCosts[building.Building] = building.OperationCost.Items;
        }
    }


    private static IReadOnlyList<SharedGoal> GetSharedGoalList(ResourcesData[] goals)
    {
        if (goals == null) return null;

        var list = new List<SharedGoal>();

        foreach (var goal in goals)
        {
            list.Add(new(goal.name, goal.Quantities.Items));
        }

        return list;
    }




    /// <summary>
    /// Call this after roles and factions have been set.
    /// Picks 3 basic resources other than traitors starting resources
    /// and 3 combined resources, one random and 2 crafted using either traitor starting resource(but not both)
    /// </summary>
    public static  SharedGoal GenerateSelfishGoal(GameInstance Game, int randomSeed, int quantity)
    {
        List<Resource> traitorsStartingResources = new List<Resource>();
        foreach ((PlayerID id, PlayerData playerData) in Game.PlayerData)
        { 
            if(playerData.Role != PlayerRole.Selfish) continue;

            traitorsStartingResources.Add(playerData.Faction.StartingResource());       
        }


        System.Random RNG = new System.Random(randomSeed);

        List<Resource> GoalsResources = new List<Resource>();

        HashSet<Resource>[] EasyCombinedResources = { new(), new() };
        foreach ((Building building, var cost) in OperationCosts) { 
            if(cost.Count == 0) continue;
            foreach((Resource resource, int n) in cost)
            {
                for(int i = 0; i < 2; i++)
                {
                    if (resource == traitorsStartingResources[i])
                        EasyCombinedResources[i].Add(building.ResourceYieldType());
                }
            }
        }

        for(int i = 0;i < 2; i++)
        {
            var Easy = EasyCombinedResources[i].Except(EasyCombinedResources[(i+1)%2]).ToList();
            Easy.Sort();//because hashset order is not deterministic
            GoalsResources.Add(Easy[RNG.Next(Easy.Count)]);
        }

        var Hard = ResourceExtention.GetAllCombined().Except(GoalsResources).ToArray();
        GoalsResources.Add(Hard[RNG.Next(Hard.Count())]);

        for(int i = 0; i <3 ; i++)
        {
            var Basics = ResourceExtention.GetAllBasic().Except(traitorsStartingResources).Except(GoalsResources).ToList();
            GoalsResources.Add(Basics[RNG.Next(Basics.Count())]);
        }

        Dictionary<Resource, int> Required = new();

        foreach (Resource res in GoalsResources)
        {
            Required[res] = quantity;
        }

        return new SharedGoal("Seed Chaos", Required);
    }
}
