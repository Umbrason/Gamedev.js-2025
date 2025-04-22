using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Resource resource;
    public Resource Resource
    {
        get => resource;
        set
        {
            if (value == resource) return;
            resourceIcon.sprite = ResourceIcons[value];
            resource = value;
            tooltip.Setup(ResourceIcons[value].name);
        }
    }

    private int amount;
    public int Amount
    {
        get => amount;
        set
        {
            if (value == amount) return;
            amount = value;
            text.text = $"{amount}";
        }
    }

    [SerializeField] private ResourceSpriteLib ResourceIcons;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CostsUITooltip tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.gameObject.SetActive(true);
    }
}
