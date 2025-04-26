using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcePledgeInput : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private ResourceCountInput resourceCountInputTemplate;
    private readonly Dictionary<Resource, ResourceCountInput> ResourceCountInputs = new();
    [SerializeField] private GameObject BalancedTheme;
    [SerializeField] private GameObject EvilTheme;
    [SerializeField] private TMP_Text Title;
    private SharedGoalID m_SharedGoalID;
    public SharedGoalID SharedGoalID
    {
        get => m_SharedGoalID;
        set
        {
            m_SharedGoalID = value;
            BalancedTheme.SetActive(value.TargetRole == PlayerRole.Balanced);
            EvilTheme.SetActive(value.TargetRole == PlayerRole.Selfish);
        }
    }
    private SharedGoal m_SharedGoal;
    public SharedGoal SharedGoal
    {
        get => m_SharedGoal;
        set
        {
            Title.text = value?.Name;
            m_SharedGoal = value;
            if (Game) Game.ClientPlayerData.OnResourceChanged -= OnPlayerResourcesChanged;
            foreach (var instance in ResourceCountInputs.Values)
                Destroy(instance.gameObject);
            ResourceCountInputs.Clear();
            if (value == null) return;
            foreach (var (resource, cost) in value.Required)
            {
                var instance = Instantiate(resourceCountInputTemplate, container);
                var available = Game.ClientPlayerData.Resources.GetValueOrDefault(resource) + CurrentPledge.GetValueOrDefault(resource);
                var required = cost - CurrentPledge.GetValueOrDefault(resource) - value.Collected.GetValueOrDefault(resource);
                instance.Max = Mathf.Min(available, required);
                instance.Resource = resource;
                instance.Value = CurrentPledge.GetValueOrDefault(resource);
                instance.OnChanged += (amount) =>
                {
                    var diff = amount - CurrentPledge.GetValueOrDefault(resource);
                    CurrentPledge[resource] = amount;
                    OnPledgeResources?.Invoke(SharedGoalID, resource, diff);
                };
                ResourceCountInputs[resource] = instance;
            }
            if (Game) Game.ClientPlayerData.OnResourceChanged += OnPlayerResourcesChanged;
        }
    }
    public Dictionary<Resource, int> CurrentPledge { get; } = new();
    public event Action<SharedGoalID, Resource, int> OnPledgeResources;
    public GameInstance Game { get; set; }

    void OnPlayerResourcesChanged(Resource r, int amount)
    {
        foreach (var (resource, instance) in ResourceCountInputs)
        {
            var cost = SharedGoal?.Required?.GetValueOrDefault(resource) ?? 0;
            var available = Game.ClientPlayerData.Resources.GetValueOrDefault(resource) + CurrentPledge.GetValueOrDefault(resource);
            var required = cost - SharedGoal?.Collected?.GetValueOrDefault(resource) ?? 0;
            instance.Max = Mathf.Min(available, required);
        }
    }

    void OnDestroy()
    {
        if (Game) Game.ClientPlayerData.OnResourceChanged -= OnPlayerResourcesChanged;
    }
}