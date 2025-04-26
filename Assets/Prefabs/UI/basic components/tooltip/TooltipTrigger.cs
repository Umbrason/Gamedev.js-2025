using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    //Put this script on a gamobject that triggers a tooltip when you hover over it.


    [TextArea]
    public string tooltipText;

    private GameObject currentTooltip;
    private static GameObject tooltipPrefab;
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Auto-load the prefab from Resources the first time it's needed
        if (tooltipPrefab == null)
        {
            tooltipPrefab = Resources.Load<GameObject>("TooltipPrefab");
            if (tooltipPrefab == null)
            {
                Debug.LogError("Tooltip prefab not found! Make sure it's at: Resources/TooltipPrefab.prefab");
                return;
            }
        }

        // Spawn tooltip under the root Canvas
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in parents for tooltip!");
            return;
        }

        currentTooltip = Instantiate(tooltipPrefab, canvas.transform);
        
        TooltipUI tooltipUI = currentTooltip.GetComponent<TooltipUI>();
        if (tooltipUI != null)
        {
            tooltipUI.SetText(tooltipText);
            tooltipUI.FollowMouse = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
        }
    }
}
