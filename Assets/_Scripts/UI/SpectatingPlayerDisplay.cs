using System.Collections;
using UnityEngine;

public class SpectatingPlayerDisplay : MonoBehaviour
{
    [Tooltip("Make sure to assign in correct order! (see PlayerFactions enum)")]
    [SerializeField] private GameObject[] factionAnim;
    private PlayerFactions currentSpectator = PlayerFactions.None;

    public void Setup(PlayerFactions faction)
    {
        currentSpectator = faction;
        UpdateAnim();
    }

    public void RemoveSpectator()
    {
        currentSpectator = PlayerFactions.None;
        UpdateAnim();
    }

    private void UpdateAnim()
    {
        for (int i = 0; i < factionAnim.Length; i++)
        {
            factionAnim[i].SetActive((int)currentSpectator == i + 1);   // already accounts for None
        }
    }
}
