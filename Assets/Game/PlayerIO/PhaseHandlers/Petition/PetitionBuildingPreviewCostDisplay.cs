using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PetitionBuildingPreviewCostDisplay : MonoBehaviour
{
    BuildingPetition m_Petition;
    public BuildingPetition Petition
    {
        get => m_Petition;
        set
        {
            m_Petition = value;
            Clear();
            if (value == null) return;
            Instantiate();

        }
    }

    public void Refresh()
    {
        foreach (var (resource, cost) in Petition.Building.ConstructionCosts())
        {
            if (!ResourceCostItems.TryGetValue(resource, out var instance))
                ResourceCostItems[resource] = instance = Instantiate(Template, container);
            instance.Provided = Petition.ResourceSources.Sum(p => p.Value?.GetValueOrDefault(resource) ?? 0);
            instance.Required = cost;
            instance.Resource = resource;
        }
    }

    private void Instantiate()
    {
        Clear();
        foreach (var (resource, cost) in Petition.Building.ConstructionCosts())
        {
            var instance = Instantiate(Template, container);
            ResourceCostItems[resource] = instance;
            instance.Provided = Petition.ResourceSources.Sum(p => p.Value?.GetValueOrDefault(resource) ?? 0);
            instance.Required = cost;
            instance.Resource = resource;
        }
    }

    private void Clear()
    {
        foreach (var instance in ResourceCostItems.Values)
            Destroy(instance.gameObject);
        ResourceCostItems.Clear();
    }

    [SerializeField] private PetitionResourceCostItem Template;
    private Dictionary<Resource, PetitionResourceCostItem> ResourceCostItems = new();
    [SerializeField] private Transform container;
    [SerializeField] private Transform target;
    [SerializeField] private Camera canvasCamera;
    [SerializeField] private Canvas canvas;

    void Update()
    {
        var screenPoint = canvasCamera.WorldToScreenPoint(target.position);
        var pos = canvas.pixelRect.size * screenPoint / new Vector3(Screen.width, Screen.height, 0);
        transform.position = pos;
    }
}
