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

    private static GameSettings activeSettings = null;

    public static IReadOnlyList<SharedGoal> BalanceGoals { get; private set; }
    public static IReadOnlyList<SharedGoal> SelfishGoals { get; private set; }
    public static Dictionary<Building, Dictionary<Resource, int>> ConstructionCosts { get; private set; }
    public static Dictionary<Building, Dictionary<Resource, int>> OperationCosts { get; private set; }
    public static FactionData[] Factions => activeSettings?.factions;

    public void Use()
    {
        BalanceGoals = GetSharedGoalList(balancedGoals);
        SelfishGoals = GetSharedGoalList(selfishGoals);

        ConstructionCosts = new();
        OperationCosts = new();
        foreach (BuildingData building in buildings)
        {
            ConstructionCosts[building.Building] = building.Cost.Items;
            OperationCosts[building.Building] = building.OperationCost.Items;
        }

        activeSettings = this;
    }


    private IReadOnlyList<SharedGoal> GetSharedGoalList(ResourcesData[] goals)
    {
        if (goals == null) return null;

        var list = new List<SharedGoal>();

        foreach (var goal in goals) {
            list.Add(new(goal.name, goal.Quantities.Items));
        }

        return list;
    }
}
