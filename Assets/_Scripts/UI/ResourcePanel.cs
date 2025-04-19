using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResourcePanel : MonoBehaviour
{

    [SerializeField] private GameInstance gameInstance;
    [SerializeField] private Transform resourceParent;
    [SerializeField] private ResourceItem resourceItem;


    PlayerData testingData = new PlayerData();

    public bool testing;

    private void Start()
    {
        if (!gameInstance) gameInstance = gameObject.GetComponent<GameInstance>();


        testingData.Resources = new Dictionary<Resource, int>
        {
            { Resource.Dewdrops, 2 },
            { Resource.Leaves, 1 },
            { Resource.Earth, 5 },
            { Resource.Mana, 0 },
            { Resource.Wood, 0 },
            { Resource.Fireflies, 7 }
        };

        UpdateResources();
    }

    private void UpdateResources()
    {
        if (resourceParent == null) return;

        PlayerData playerData = testing ? testingData : gameInstance.ClientPlayerData;

        if(resourceParent.childCount > 0) resourceParent.DeleteChildren();

        foreach (var (resource, amount) in playerData.Resources)
        {
            // spawn resourceItem
            ResourceItem item = Instantiate(resourceItem, resourceParent);
            item.Init(resource, amount);
            item.UpdateUI();
        }
    }
}
