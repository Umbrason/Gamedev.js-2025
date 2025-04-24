using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerResourceSourceDisplay : MonoBehaviour
{
    [SerializeField] private ResourceSpriteLib ResourceIcons;
    [SerializeField] private PlayerFactionSpriteLib FactionIcons;
    [SerializeField] private Image FactionIcon;
    [SerializeField] private Transform ResourceContainer;
    [SerializeField] private Image ResourceIconTemplate;
    private readonly List<Image> instances = new();
    private PlayerFaction m_Faction;
    public PlayerFaction Faction
    {
        get => m_Faction;
        set
        {
            m_Faction = value;
            FactionIcon.sprite = FactionIcons[value];
        }
    }

    private Dictionary<Resource, int> m_Resources;
    public Dictionary<Resource, int> Resources
    {
        get => m_Resources;
        set
        {
            m_Resources = value;
            foreach (var i in instances)
                Destroy(i.gameObject);
            instances.Clear();
            if (value == null) return;
            foreach (var (resource, amount) in value)
            {
                for (int i = 0; i < amount; i++)
                {
                    var instance = Instantiate(ResourceIconTemplate, ResourceContainer);
                    instance.sprite = ResourceIcons[resource];
                    instances.Add(instance);
                }
            }
        }
    }
}