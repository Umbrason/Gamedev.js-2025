using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceItem : MonoBehaviour
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
}
