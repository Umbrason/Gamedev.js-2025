using System;
using System.Linq;
using UnityEngine;

public class MissionsDisplay : MonoBehaviour
{
    [SerializeField] private GameInstance Game;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private SharedGoalsDisplay balancedGoalDisplay;
    [SerializeField] private SharedGoalsDisplay selfishGoalDisplay;
    [SerializeField] private SecretTaskDisplay secretTaskDisplay;
    public event Action<SharedGoalID> OnClickMission;

    public SharedGoalDisplay ButtonOf(SharedGoalID goalID)
    {
        var display = goalID.TargetRole switch
        {
            PlayerRole.Balanced => balancedGoalDisplay,
            PlayerRole.Selfish => selfishGoalDisplay,
            _ => null
        };
        return display?.ButtonOf(goalID);
    }

    public void Show()
    {
        Canvas.SetActive(true);
        var balancedGoals = Game.BalancedFactionGoals.Select((goal, index) => (goal, new SharedGoalID(PlayerRole.Balanced, index))).ToDictionary(p => p.Item2, p => p.goal);
        var selfishGoals = Game.ClientPlayerData.Role == PlayerRole.Selfish ? Game.SelfishFactionGoals.Select((goal, index) => (goal, new SharedGoalID(PlayerRole.Selfish, index))).ToDictionary(p => p.Item2, p => p.goal) : null;
        balancedGoalDisplay.Goals = balancedGoals;
        selfishGoalDisplay.Goals = selfishGoals;
        balancedGoalDisplay.OnClick = OnClick;
        selfishGoalDisplay.OnClick = OnClick;
        secretTaskDisplay.SecretTask = Game.ClientPlayerData.SecretGoal;
    }
    public void Hide()
    {
        Canvas.SetActive(false);
    }

    void OnClick(SharedGoalID sharedGoalID) => OnClickMission?.Invoke(sharedGoalID);
}
