using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SharedGoalDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject BalancedTheme;
    [SerializeField] private GameObject EvilTheme;
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Status;
    private SharedGoalID m_SharedGoalID;
    public SharedGoalID GoalID
    {
        get => m_SharedGoalID;
        set
        {
            m_SharedGoalID = value;
            BalancedTheme.SetActive(value.TargetRole == PlayerRole.Balanced);
            EvilTheme.SetActive(value.TargetRole == PlayerRole.Selfish);
        }
    }

    private SharedGoal m_Goal;
    public SharedGoal Goal
    {
        get => m_Goal;
        set
        {
            m_Goal = value;
            var progressLines = new List<string>();
            foreach (var (resource, required) in value.Required)
            {
                //TODO: replace this with a proper prefab
                progressLines.Add($"{resource}:{m_Goal.Collected.GetValueOrDefault(resource)}/{required}");
            }
            Title.text = value.Name;
            Status.text = string.Join(' ', progressLines);
        }
    }
    public Action<SharedGoalID> OnClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(GoalID);
    }
}