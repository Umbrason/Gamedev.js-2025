using System.Collections.Generic;
using DataView;
using UnityEngine;

public class PlayerIslandViewer : MonoBehaviour
{
    [SerializeField] private GameInstance Game;
    private readonly Dictionary<PlayerFaction, IslandView> viewInstances = new();
    [SerializeField] List<IslandView> viewTemplates;

    private void Awake()
    {
        for (int i = 0; i < 6; i++)
        {
            var slot = (PlayerFaction)(i + 1);
            var instance = Instantiate(viewTemplates[i], transform);
            viewInstances[slot] = instance;
            instance.gameObject.SetActive(false);
        }
    }

    void UpdateIsland(PlayerIsland island) => viewInstances[Game.PlayerData[m_TargetPlayer].Faction].Data = island;

    private PlayerID m_TargetPlayer;
    public PlayerID TargetPlayer
    {
        get => m_TargetPlayer;
        set
        {
            if (m_TargetPlayer != PlayerID.None) Game.PlayerData[m_TargetPlayer].OnIslandChanged -= UpdateIsland;
            m_TargetPlayer = value;
            if (m_TargetPlayer != PlayerID.None) Game.PlayerData[m_TargetPlayer].OnIslandChanged += UpdateIsland;
            var playerFaction = Game.PlayerData[value].Faction;
            foreach (var (id, instance) in viewInstances)
            {
                instance.gameObject.SetActive(id == Game.PlayerData[value].Faction);
                if (id == playerFaction) instance.Data = Game.PlayerData[value].Island;
            }
        }
    }
}
