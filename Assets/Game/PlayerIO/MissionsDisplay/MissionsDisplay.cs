using System;
using System.Linq;
using UnityEngine;

public class MissionsDisplay : MonoBehaviour
{
    [SerializeField] private GameInstance Game;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private SharedGoalsDisplay balancedGoalDisplayTemplate;
    [SerializeField] private SharedGoalsDisplay selfishGoalDisplayTemplate;
    [SerializeField] private SecretTaskDisplay secretTaskDisplay;
    public event Action<SharedGoalID> OnClickMission;

    public void Show()
    {
        Canvas.SetActive(true);
        var balancedGoals = Game.BalancedFactionGoals.Select((goal, index) => (goal, new SharedGoalID(PlayerRole.Balanced, index))).ToDictionary(p => p.Item2, p => p.goal);
        var selfishGoals = Game.ClientPlayerData.Role == PlayerRole.Selfish ? Game.SelfishFactionGoals.Select((goal, index) => (goal, new SharedGoalID(PlayerRole.Selfish, index))).ToDictionary(p => p.Item2, p => p.goal) : null;
        balancedGoalDisplayTemplate.Goals = balancedGoals;
        selfishGoalDisplayTemplate.Goals = selfishGoals;
        balancedGoalDisplayTemplate.OnClick = OnClick;
        selfishGoalDisplayTemplate.OnClick = OnClick;
        secretTaskDisplay.SecretTask = Game.ClientPlayerData.SecretGoal;
    }
    public void Hide()
    {
        Canvas.SetActive(false);
    }

    void OnClick(SharedGoalID sharedGoalID) => OnClickMission?.Invoke(sharedGoalID);
}
