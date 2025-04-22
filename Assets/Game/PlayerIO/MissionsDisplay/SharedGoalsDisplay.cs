using System;
using System.Collections.Generic;
using UnityEngine;

public class SharedGoalsDisplay : MonoBehaviour
{
    [SerializeField] Transform container;
    Dictionary<SharedGoalID, SharedGoal> m_Goals;
    public Dictionary<SharedGoalID, SharedGoal> Goals
    {
        get => m_Goals;
        set
        {
            m_Goals = value;
            foreach (var instance in GoalDisplayInstances.Values)
                Destroy(instance.gameObject);
            GoalDisplayInstances.Clear();
            if (value == null) return;
            foreach (var (id, goal) in value)
            {
                var instance = Instantiate(Template, container);
                instance.GoalID = id;
                instance.Goal = goal;
                instance.OnClick = (id) => OnClick?.Invoke(id);
                GoalDisplayInstances[id] = instance;
            }
        }
    }
    private readonly Dictionary<SharedGoalID, SharedGoalDisplay> GoalDisplayInstances = new();
    [SerializeField] private SharedGoalDisplay Template;
    public Action<SharedGoalID> OnClick;
}