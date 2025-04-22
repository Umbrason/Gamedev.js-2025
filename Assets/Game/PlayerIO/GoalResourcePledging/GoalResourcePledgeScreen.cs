using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalResourcePledgeScreen : MonoBehaviour
{
    [SerializeField] private GameInstance game;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private Transform container;
    [SerializeField] private ResourcePledgeInput ResourcePledgeInputTemplate;
    private readonly Dictionary<SharedGoalID, ResourcePledgeInput> ResourcePledgeInputs = new();

    public void Show(Dictionary<SharedGoalID, SharedGoal> goals, Action<SharedGoalID, Resource, int> OnPledgeResource)
    {
        foreach (var (_, instance) in ResourcePledgeInputs)
            Destroy(instance.gameObject);
        ResourcePledgeInputs.Clear();
        foreach (var (id, goal) in goals)
        {
            var instance = Instantiate(ResourcePledgeInputTemplate, container);
            instance.OnPledgeResources += OnPledgeResource;
            instance.game = game;
            instance.SharedGoalID = id;
            instance.SharedGoal = goal;
            ResourcePledgeInputs[id] = instance;
        }
    }

    public void Hide()
    {
        Canvas.SetActive(false);
    }
}
