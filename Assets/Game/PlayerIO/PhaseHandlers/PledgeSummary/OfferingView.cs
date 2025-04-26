using DataView;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OfferingView : MonoBehaviour
{
    [SerializeField] ResourceItem resourcesItemTemplate;
    [SerializeField] Transform Container;
    private readonly List<ResourceItem> resourcesItemInstances = new();
    [SerializeField] PlayerIDButton playerIDButton;
    [SerializeField] GameObject NothingContributed;

    public PlayerFaction Faction
    {
        set => playerIDButton.Faction = value;
    }
    public string Nickname
    {
        set => playerIDButton.Nickname = value;
    }

    public Dictionary<Resource, int> OfferedResources
    {
        set
        {
            foreach (var i in resourcesItemInstances) Destroy(i.gameObject);
            resourcesItemInstances.Clear();
            var isNothing = value == null || value?.Values?.Sum() == 0;
            NothingContributed.SetActive(isNothing);
            if (isNothing) return;
            foreach ((Resource res, int quantity) in value)
            {
                if (quantity == 0) continue;
                var instance = Instantiate(resourcesItemTemplate, Container);
                instance.Resource = res;
                instance.Amount = quantity;
                resourcesItemInstances.Add(instance);
            }
        }
    }
}
