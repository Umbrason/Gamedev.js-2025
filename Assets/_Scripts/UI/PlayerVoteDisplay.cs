using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// is instantiated when a player voted. the Setup is called with the data of the vote.
/// </summary>
public class PlayerVoteDisplay : MonoBehaviour
{
    [SerializeField] private Image factionIcon;
    [SerializeField] private PlayerFactionSpriteLib factionSprites;


    // TODO: @Cathy are u fine with this?
    public void Setup(PlayerFaction faction, int vote)
    {
        factionIcon.sprite = factionSprites[faction];
    }
}
