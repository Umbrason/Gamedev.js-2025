using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetitionResourceCostItem : MonoBehaviour
{
    [SerializeField] private ResourceSpriteLib ResourceIcons;
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TMP_Text provided;
    [SerializeField] private TMP_Text required;

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


    private int m_Required;
    public int Required
    {
        get => m_Required;
        set
        {
            m_Required = value;
            required.text = $"{value}";
        }
    }

    private int m_Provided;
    public int Provided
    {
        get => m_Provided;
        set
        {
            m_Provided = value;
            provided.text = $"{value}";
        }
    }
}
