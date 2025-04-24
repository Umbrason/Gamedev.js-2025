using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AccusationVoteDialogue : MonoBehaviour
{
    [SerializeField] private GameInstance game;
    [SerializeField] private PlayerFactionSpriteLib factionIcons;
    [SerializeField] private Image[] Icons;

    [SerializeField] private Button Agree;
    [SerializeField] private Button Disagree;

    [SerializeField] private GameObject AccusedSelf;
    [SerializeField] private GameObject OthersAccused;
    public event Action<bool> OnVote;

    void Awake()
    {
        Agree.onClick.AddListener(() => OnVote?.Invoke(true));
        Agree.onClick.AddListener(() => Disagree.interactable = Agree.interactable = false);
        Disagree.onClick.AddListener(() => OnVote?.Invoke(false));
        Disagree.onClick.AddListener(() => Disagree.interactable = Agree.interactable = false);
    }

    public void Show(PlayerID[] accused)
    {
        gameObject.SetActive(true);
        Agree.interactable = true;
        Disagree.interactable = true;
        var clientAccused = accused.Contains(game.ClientID);
        AccusedSelf.SetActive(clientAccused);
        OthersAccused.SetActive(!clientAccused);
        for (int i = 0; i < accused.Length; i++)
            Icons[i].sprite = factionIcons[game.PlayerData[accused[i]].Faction];
    }
}