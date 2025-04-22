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
        for (int i = 0; i < NetworkUtils.playerCount; i++)
        {
            var slot = (PlayerID)i;
            var instance = Instantiate(viewTemplates[i], transform);
            viewInstances[slot] = instance;
            instance.gameObject.SetActive(false);
        }
    }

    void UpdateIsland(PlayerIsland island) => viewInstances[m_TargetPlayer].Data = island;

    private PlayerID m_TargetPlayer;
    public PlayerID TargetPlayer
    {
        get => m_TargetPlayer;
        set
        {
            if (m_TargetPlayer != PlayerID.None) Game.PlayerData[m_TargetPlayer].OnIslandChanged -= UpdateIsland;
            m_TargetPlayer = value;
            if (m_TargetPlayer != PlayerID.None) Game.PlayerData[m_TargetPlayer].OnIslandChanged += UpdateIsland;
            foreach (var (id, instance) in viewInstances)
            {
                instance.gameObject.SetActive(id == value);
                instance.Data = Game.PlayerData[id].Island;
            }
        }
    }
}
