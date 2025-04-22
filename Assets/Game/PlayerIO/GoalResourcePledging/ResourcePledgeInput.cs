using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePledgeInput : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private ResourceCountInput resourceCountInputTemplate;
    private readonly Dictionary<Resource, ResourceCountInput> ResourceCountInputs = new();

    public SharedGoalID SharedGoalID { get; set; }
    private SharedGoal m_SharedGoal;
    public SharedGoal SharedGoal
    {
        get => m_SharedGoal;
        set
        {
            m_SharedGoal = value;
            foreach (var instance in ResourceCountInputs.Values)
                Destroy(instance.gameObject);
            ResourceCountInputs.Clear();
            if (value == null) return;
            foreach (var (resource, cost) in value.Required)
            {
                var instance = Instantiate(resourceCountInputTemplate, container);
                var available = game.ClientPlayerData.Resources.GetValueOrDefault(resource) + currentPledge[resource];
                var required = cost - currentPledge.GetValueOrDefault(resource) - value.Collected.GetValueOrDefault(resource);
                instance.Max = Mathf.Min(available, required);
                instance.OnChanged += (amount) =>
                {
                    var diff = amount - currentPledge.GetValueOrDefault(resource);
                    currentPledge[resource] = amount;
                    OnPledgeResources?.Invoke(SharedGoalID, resource, diff);
                };
                ResourceCountInputs[resource] = instance;
            }
            if (game) game.ClientPlayerData.OnResourceChanged += OnPlayerResourcesChanged;
        }
    }
    private Dictionary<Resource, int> currentPledge = new();
    public event Action<SharedGoalID, Resource, int> OnPledgeResources;
    public GameInstance game;

    void OnPlayerResourcesChanged(Resource r, int amount)
    {
        foreach (var (resource, instance) in ResourceCountInputs)
        {
            var cost = SharedGoal?.Required?.GetValueOrDefault(resource) ?? 0;
            var available = game.ClientPlayerData.Resources.GetValueOrDefault(resource) + currentPledge[resource];
            var required = cost - currentPledge.GetValueOrDefault(resource) - SharedGoal?.Collected?.GetValueOrDefault(resource) ?? 0;
            instance.Max = Mathf.Min(available, required);
            instance.OnChanged += (amount) =>
            {
                var diff = amount - currentPledge.GetValueOrDefault(resource);
                currentPledge[resource] = amount;
                OnPledgeResources?.Invoke(SharedGoalID, resource, diff);
            };
            ResourceCountInputs[resource] = instance;
        }
    }

    void OnDestroy()
    {
        if (game) game.ClientPlayerData.OnResourceChanged -= OnPlayerResourcesChanged;
    }
}