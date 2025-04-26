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
}
