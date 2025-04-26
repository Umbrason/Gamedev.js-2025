using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerIDButtons : MonoBehaviour
{
    [SerializeField] private GameInstance game;
    [SerializeField] private PlayerIDButton playerIDButtonTemplate;
    [SerializeField] private PlayerIDButton clientPlayerIDButtonTemplate;
    private readonly List<PlayerIDButton> buttonInstances = new();
    [SerializeField] private Transform container;
    public Action<PlayerID> OnClick;

    public PlayerIDButton ButtonOf(PlayerID playerID) => buttonInstances.FirstOrDefault(button => button.PlayerID == playerID);

    public void Refresh()
    {
        Clear();
        for (PlayerID id = 0; (int)id < 6; id++)
        {
            var instance = Instantiate(id == game.ClientID ? clientPlayerIDButtonTemplate : playerIDButtonTemplate, container);
            instance.onClick += OnClickButton;
            instance.PlayerID = id;
            instance.Faction = game?.PlayerData?.GetValueOrDefault(id)?.Faction ?? PlayerFaction.None;
            instance.Nickname = game?.PlayerData?.GetValueOrDefault(id)?.Nickname ?? instance.Faction.ToString();
            instance.IsTraitor = game?.PlayerData?.GetValueOrDefault(id).Role == PlayerRole.Selfish && game?.ClientPlayerData?.Role == PlayerRole.Selfish;
            buttonInstances.Add(instance);
        }
        if (game.ClientID >= 0) buttonInstances[(int)game.ClientID].transform.SetSiblingIndex(0);
    }
    private void OnClickButton(PlayerID id) => OnClick?.Invoke(id);

    public void Clear()
    {
        foreach (var instance in buttonInstances)
            Destroy(instance.gameObject);
        buttonInstances.Clear();
    }

    void OnEnable() => Refresh();
    void OnDisable() => Clear();
}
