using System.Collections;
using UnityEngine;

public class PlayerSlot : MonoBehaviour
{
    [Tooltip("Make sure to assign in correct order! (see PlayerFactions enum)")]
    [SerializeField] private GameObject[] factionAnim;
    private PlayerFactions faction = PlayerFactions.None;
    public PlayerFactions ActiveFaction
    {
        get => faction; set
        {
            faction = value;
            UpdateAnim();
        }
    }

    private void UpdateAnim()
    {
        for (int i = 0; i < factionAnim.Length; i++)
            factionAnim[i].SetActive((int)faction == i + 1);   // already accounts for None
    }
}
