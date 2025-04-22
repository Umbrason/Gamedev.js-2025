using UnityEngine;

public class MissionsDisplay : MonoBehaviour
{
    GameInstance instance;

    [SerializeField] private SharedGoalsDisplay balancedGoalDisplayTemplate;
    [SerializeField] private SharedGoalsDisplay selfishGoalDisplayTemplate;

    public void Refresh()
    {
        balancedGoalDisplayTemplate.Goals = instance.BalancedFactionGoals;
        selfishGoalDisplayTemplate.Goals = instance.SelfishFactionGoals;
    }
}
