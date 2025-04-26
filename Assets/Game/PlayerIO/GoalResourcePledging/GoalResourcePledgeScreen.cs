using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalResourcePledgeScreen : MonoBehaviour
{
    [SerializeField] private GameInstance game;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private Transform container;
    [SerializeField] private ResourcePledgeInput ResourcePledgeInputTemplate;
    [SerializeField] private Button HideButton;
    private readonly Dictionary<SharedGoalID, ResourcePledgeInput> ResourcePledgeInputs = new();

    public bool Showing { get; private set; }

    public void Show(Dictionary<SharedGoalID, SharedGoal> goals, ResourcePledge currentPledge, Action<SharedGoalID, Resource, int> OnPledgeResource, bool CanHide = true)
    {
        if (Showing) Hide();
        Showing = true;
        HideButton.gameObject.SetActive(CanHide);
        HideButton.onClick.AddListener(Hide);
        foreach (var (_, instance) in ResourcePledgeInputs)
            Destroy(instance.gameObject);
        ResourcePledgeInputs.Clear();
        foreach (var (id, goal) in goals)
        {
            var instance = Instantiate(ResourcePledgeInputTemplate, container);
            instance.OnPledgeResources += OnPledgeResource;
            instance.Game = game;
            foreach (var (resource, amount) in currentPledge?.goalPledges?.GetValueOrDefault(id) ?? new Dictionary<Resource, int>())
                instance.CurrentPledge[resource] = amount;
            instance.SharedGoalID = id;
            instance.SharedGoal = goal;
            ResourcePledgeInputs[id] = instance;
        }
        Canvas.SetActive(true);
    }

    public void Hide()
    {
        Showing = false;
        HideButton.onClick.RemoveListener(Hide);
        Canvas.SetActive(false);
    }
}
