using DataView;
using System.Collections.Generic;
using UnityEngine;

public class PledgeView : MonoBehaviour
{
    [SerializeField] ResourceItem[] resourcesItems;
    public ResourcePledge Pledge
    {
        set
        {
            if(value == null)
            {
                gameObject.SetActive(false);
                return;
            }

            Dictionary<Resource, int> Pledge = new();

            foreach ((SharedGoalID goal, Dictionary<Resource, int> resources) in value.goalPledges)
            {
                if (goal.TargetRole == PlayerRole.Balanced) Pledge = resources;
            }

            int i = 0;
            
            bool gaveSomething = false;
            foreach ((Resource res, int quantity) in Pledge)
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
