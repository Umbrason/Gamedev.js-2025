using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildingTooltip : MonoBehaviour
{
    [SerializeField] private Transform CostsContainer;
    [SerializeField] private Transform OperationCostsContainer;
    [SerializeField] private ResourceItem Yield;
    [SerializeField] private ResourceItem ResourceTemplate;
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Description;


    readonly List<GameObject> instances = new();
    private Building m_Building;
    public Building Building
    {
        get => m_Building;
        set
        {
            m_Building = value;
            foreach (var instance in instances)
                Destroy(instance);
            instances.Clear();
            Title.text = value.Name();
            Description.text = value.Description();
            foreach (var (res, cost) in value.ConstructionCosts())
            {
                for (int i = 0; i < cost; i++)
                {
                    var instance = Instantiate(ResourceTemplate, CostsContainer);
                    instance.Resource = res;
                    instances.Add(instance.gameObject);
                }
            }

            foreach (var (res, cost) in value.OperationCosts())
            {
                for (int i = 0; i < cost; i++)
                {
                    var instance = Instantiate(ResourceTemplate, OperationCostsContainer);
                    instance.Resource = res;
                    instances.Add(instance.gameObject);
                }
            }
            Yield.Resource = value.ResourceYieldType();
        }
    }
}
