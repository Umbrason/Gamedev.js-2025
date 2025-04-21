using System.Collections.Generic;
using UnityEngine;

public class PlayerDisplayProvider : MonoBehaviour
{
    [SerializeField] private SpectatingPlayerDisplay clientDisplay;
    public SpectatingPlayerDisplay ClientDisplay { get => clientDisplay; }


    [SerializeField] private List<SpectatingPlayerDisplay> displayQueue = new List<SpectatingPlayerDisplay>();
    private readonly Dictionary<PlayerFactions, SpectatingPlayerDisplay> occupiedDisplays = new();


    public void SwitchDisplayOwner(PlayerFactions faction, bool occupying)
    {
        if (occupying) OccupyDisplay(faction);
        else ReturnDisplay(faction);
    }

    public void OccupyDisplay(PlayerFactions faction)
    {
        if (occupiedDisplays.ContainsKey(faction) || clientDisplay.Visitor == faction) return;  // each faction can only be there once
        if (displayQueue.Count <= 0)
        {
            Debug.LogWarning("PlayerDisplay was requested with none left to occupy.");
            return;
        }

        SpectatingPlayerDisplay random = displayQueue[Random.Range(0, displayQueue.Count)];
        displayQueue.Remove(random);
        occupiedDisplays.Add(faction, random);
        random.Setup(faction);
    }

    public void ReturnDisplay(PlayerFactions faction)
    {
        if (!occupiedDisplays.ContainsKey(faction))
        {
            Debug.LogWarning("Tried returning Display for Faction that wasn't visiting.");
            return;   // wasn't even visiting
        }

        occupiedDisplays[faction].RemoveSpectator();
        displayQueue.Add(occupiedDisplays[faction]);
        occupiedDisplays.Remove(faction);
    }
}
