using DataView;
using System.Collections.Generic;
using UnityEngine;

public class OfferingView : MonoBehaviour
{
    [SerializeField] ResourceItem[] resourcesItems;
    public Dictionary<Resource, int> OfferedResources
    {
        set
        {
            if(value == null)
            {
                gameObject.SetActive(false);
                return;
            }

            int i = 0;

            bool gaveSomething = false;
            foreach ((Resource res, int quantity) in value)
            {
                if (quantity <= 0) continue;
                if (i >= resourcesItems.Length)
                {
                    Debug.LogError("Not enough ResourceItem in PledgeView");
                    break;
                }

                gaveSomething = true;
                ResourceItem item = resourcesItems[i++];
                item.Resource = res;
                item.Amount = quantity;
                item.gameObject.SetActive(true);
            }

            gameObject.SetActive(gaveSomething);

            while (i < resourcesItems.Length)
            {
                resourcesItems[i++].gameObject.SetActive(false);
            }
        }
    }
}
