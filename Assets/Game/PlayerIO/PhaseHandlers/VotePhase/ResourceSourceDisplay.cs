using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceSourceDisplay : MonoBehaviour
{
    Dictionary<PlayerID, Dictionary<Resource, int>> m_Sources;
    [SerializeField] private PlayerResourceSourceDisplay Template;
    [SerializeField] private Transform container;
    readonly List<PlayerResourceSourceDisplay> instances= new();
    [SerializeField] private GameInstance game;

    public Dictionary<PlayerID, Dictionary<Resource, int>> Sources
    {
        get => m_Sources;
        set
        {
            foreach (var i in instances)
                Destroy(i.gameObject);
            instances.Clear();
            foreach (var (player, resources) in value)
            {
                var faction = game.PlayerData[player].Faction;
                var instance = Instantiate(Template, container);
                instance.Resources = resources;
                instance.Faction = faction;
                instances.Add(instance);
            }
        }
    }
}
