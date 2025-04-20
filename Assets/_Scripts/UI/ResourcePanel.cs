using System.Collections.Generic;
using UnityEngine;

public class ResourcePanel : MonoBehaviour
{

    [SerializeField] private GameInstance gameInstance;
    [SerializeField] private Transform resourceParent;
    [SerializeField] private ResourceItem resourceItem;

    private readonly Dictionary<Resource, ResourceItem> Instances = new();

    private void Start()
    {
        if (!gameInstance) gameInstance = gameObject.GetComponent<GameInstance>();
        SpawnResourceItems();
    }
    void SpawnResourceItems()
    {
        foreach (var resource in (Resource[])System.Enum.GetValues(typeof(Resource)))
        {
            if (resource == Resource.None) continue;
            Instances[resource] = Instantiate(resourceItem, resourceParent);
            Instances[resource].Resource = resource;
            Instances[resource].Amount = 0;
        }
    }

    private void Update()
    {
        if (gameInstance?.ClientPlayerData?.Resources == null) return;
        foreach (var (resource, amount) in gameInstance.ClientPlayerData.Resources)
            Instances[resource].Amount = amount;
    }
}
