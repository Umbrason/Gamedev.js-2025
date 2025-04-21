using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerResourceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PlayerID TargetPlayer { get; set; }
    public BuildingPetition m_ActivePetition;
    public BuildingPetition ActivePetition
    {
        get => m_ActivePetition;
        set
        {
            foreach (var instance in ResourceInputInstances.Values)
                Destroy(instance.gameObject);
            ResourceInputInstances.Clear();

            foreach (var (resource, cost) in value.Building.ConstructionCosts())
            {
                var instance = Instantiate(ResourceCountInputTemplate, resourceCountInputContainer);
                instance.Max = cost;
                instance.OnChanged += (amount) => OnResourceInputChanged(resource, amount);
                ResourceInputInstances[resource] = instance;
            }
        }
    }
    public event Action<Resource, int> OnResourceCounterChanged;


    void OnResourceInputChanged(Resource resource, int amount)
    {
        OnResourceCounterChanged?.Invoke(resource, amount);
        if (amount == 0 && TinyResourceIconInstances.ContainsKey(resource))
        {
            Destroy(TinyResourceIconInstances[resource].gameObject);
            TinyResourceIconInstances.Remove(resource);
        }
        if (amount > 0)
        {
            if (!TinyResourceIconInstances.ContainsKey(resource))
                TinyResourceIconInstances[resource] = Instantiate(TinyResourceIconTemplate, tinyIconContainer);
            var tinyIconOrder = TinyResourceIconInstances.OrderBy(p => ResourceInputInstances[p.Key].Value + ((int)p.Key / (float)Enum.GetValues(typeof(Resource)).Length));
            foreach (var tinyIcon in tinyIconOrder)
                tinyIcon.Value.transform.SetSiblingIndex(0);
        }
    }

    public readonly Dictionary<Resource, ResourceCountInput> ResourceInputInstances = new();
    public readonly Dictionary<Resource, Image> TinyResourceIconInstances = new();
    [SerializeField] private PlayerIDSpriteLib playerIcons;
    [SerializeField] private ResourceSpriteLib resourceIcons;
    [SerializeField] private ResourceCountInput ResourceCountInputTemplate;
    [SerializeField] private Image TinyResourceIconTemplate;
    [SerializeField] private GameObject foldout;

    [SerializeField] private Transform tinyIconContainer;
    [SerializeField] private Transform resourceCountInputContainer;

    public void OnPointerEnter(PointerEventData eventData)
    {
        foldout.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foldout.SetActive(false);
    }
}
