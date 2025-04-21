using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIDButton : MonoBehaviour
{
    public PlayerID PlayerID { get; set; }
    PlayerFactions m_faction;
    [SerializeField] private PlayerFactionSpriteLib factionSprites;
    public PlayerFactions Faction
    {
        get => m_faction;
        set
        {
            if (m_faction == value) return;
            m_faction = value;
            icon.sprite = factionSprites[value];
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
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text text;
    public event Action<PlayerID> onClick;
    void Awake() => button.onClick.AddListener(() => onClick?.Invoke(PlayerID));
}
