using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIDToggle : MonoBehaviour
{
    public PlayerID PlayerID { get; set; }
    PlayerFaction m_faction;
    [SerializeField] private PlayerFactionSpriteLib factionSprites;
    public PlayerFaction Faction
    {
        get => m_faction;
        set
        {
            if (m_faction == value) return;
            m_faction = value;
            if (factionSprites != null) icon.sprite = factionSprites[value];
        }
    }
    private string m_Nickname;
    public string Nickname
    {
        get => m_Nickname;
        set
        {
            if (m_Nickname == value) return;
            m_Nickname = value;
            text.text = value;
        }
    }
    [SerializeField] private Image icon;
    [SerializeField] private Toggle toggle;
    [SerializeField] private TMP_Text text;
    public bool isOn
    {
        get => toggle.isOn;
        set => toggle.isOn = value;
    }
    public void SetIsOnNoNotify(bool isOn) => toggle.SetIsOnWithoutNotify(isOn);
    public event Action<PlayerID, bool> onValueChanged;
    void Awake() => toggle.onValueChanged.AddListener((value) => onValueChanged?.Invoke(PlayerID, value));
}
