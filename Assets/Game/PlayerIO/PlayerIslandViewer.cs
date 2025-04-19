using System.Collections.Generic;
using DataView;
using UnityEngine;

public class PlayerIslandViewer : MonoBehaviour
{
    [SerializeField] private GameInstance Game;
    private readonly Dictionary<PlayerID, IslandView> viewInstances = new();

    [SerializeField] List<IslandView> viewTemplates;

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            var slot = (PlayerID)i;
            var instance = Instantiate(viewTemplates[i], transform);
            viewInstances[slot] = instance;
            instance.gameObject.SetActive(false);
        }
    }

    private PlayerID m_TargetPlayer;
    public PlayerID TargetPlayer
    {
        get => m_TargetPlayer;
        set
        {
            m_TargetPlayer = value;
            foreach (var (id, instance) in viewInstances)
            {
                instance.gameObject.SetActive(id == value);
                instance.Data = Game.PlayerData[id].Island;
            }
        }
    }
}
