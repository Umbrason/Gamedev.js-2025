using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccusationPickerDialogue : MonoBehaviour
{
    [SerializeField] private GameInstance game;
    [SerializeField] private Button SkipButton;
    [SerializeField] private Button AccuseButton;

    [SerializeField]
    private TMPro.TMP_Text dilaogText;

    [SerializeField] private PlayerIDToggle playerIDToggle;
    private readonly Dictionary<PlayerID, PlayerIDToggle> playerIDToggles = new();
    [SerializeField] private Transform playerButtonContainer;
    private readonly List<PlayerID> accusation = new();

    private bool alredyMadeAnAccusation;

    public event Action<PlayerID[]> OnAccusationMade;
    void Awake()
    {
        SkipButton.onClick.AddListener(OnSkip);
        AccuseButton.onClick.AddListener(OnAccuse);
    }

    private void OnAccuse()
    {
        if (accusation.Count != 2) return;
        OnAccusationMade?.Invoke(accusation.ToArray());
    }

    private void OnSkip()
    {
        OnAccusationMade?.Invoke(null);
    }

    public void OnEnable()
    {
        ClearButtons();
        for (PlayerID id = PlayerID.Zero; (int)id < 6; id++)
        {
            if (id == game.ClientID) continue;
            var instance = Instantiate(playerIDToggle, playerButtonContainer);
            instance.PlayerID = id;
            instance.Faction = game.PlayerData[id].Faction;
            instance.Nickname = game?.PlayerData?.GetValueOrDefault(id)?.Nickname ?? instance.Faction.ToString();
            playerIDToggles.Add(id, instance);
            instance.onValueChanged += OnPlayerClicked;
        }
    }

    private void ClearButtons()
    {
        foreach (var button in playerIDToggles)
            Destroy(button.Value.gameObject);
        playerIDToggles.Clear();
    }

    void OnPlayerClicked(PlayerID player, bool picked)
    {
        if (picked)
        {
            if (accusation.Contains(player)) return;
            accusation.Add(player);
            if (accusation.Count > 2)
            {
                playerIDToggles[accusation[0]].SetIsOnNoNotify(false);
                accusation.RemoveAt(0);
            }
            playerIDToggles[player].SetIsOnNoNotify(true);
        }
        else
        {
            if (!accusation.Contains(player)) return;
            accusation.Remove(player);
            playerIDToggles[player].SetIsOnNoNotify(false);
        }
        if(!alredyMadeAnAccusation)
            AccuseButton.interactable = accusation.Count == 2 && !alredyMadeAnAccusation;
    }

    public void DisableAccusationOpprtunity()
    {
        dilaogText.text = "Accuse two players of being evil?\n<size=75%>You have already accused a player in this game</size>";
        AccuseButton.interactable = false;
        alredyMadeAnAccusation = true;
    }
}