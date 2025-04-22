using System.Collections.Generic;
using UnityEngine;

public class SharedGoalsDisplay : MonoBehaviour
{
    IReadOnlyList<SharedGoal> m_Goals;
    public IReadOnlyList<SharedGoal> Goals
    {
        get => m_Goals;
        set
        {
            m_Goals = value;
            while (value.Count != GoalDisplayInstances.Count)
            {
                if (value.Count > GoalDisplayInstances.Count)
                    GoalDisplayInstances.Add(Instantiate(Template));
                else
                {
                    Destroy(GoalDisplayInstances[0]);
                    GoalDisplayInstances.RemoveAt(0);
                }
            }
            for (int i = 0; i < value.Count; i++)
                GoalDisplayInstances[i].Goal = Goals[i];
        }
    }
    private readonly List<SharedGoalDisplay> GoalDisplayInstances = new();
    [SerializeField] private SharedGoalDisplay Template;
}