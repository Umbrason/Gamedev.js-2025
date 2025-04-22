using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private PlayerSlot clientDisplay;
    [SerializeField] private List<PlayerSlot> displaySlots = new List<PlayerSlot>();
    readonly SortedList<int, PlayerSlot> availableSlots = new();
    private readonly Dictionary<PlayerFactions, PlayerSlot> occupiedDisplays = new();

    PlayerFactions m_IslandOwner;
    public PlayerFactions IslandOwner
    {
        get => m_IslandOwner;
        set
        {
            m_IslandOwner = value;
            clientDisplay.ActiveFaction = value;
        }
    }

    void Awake()
    {
        for (int i = 0; i < displaySlots.Count; i++)
            availableSlots.Add(i, displaySlots[i]);
    }

    public void Show(PlayerFactions faction)
    {
        if (occupiedDisplays.ContainsKey(faction) || clientDisplay.ActiveFaction == faction) return;  // each faction can only be there once
        if (displaySlots.Count <= 0)
        {
            Debug.LogWarning("PlayerDisplay was requested with none left to occupy.");
            return;
        }

        //PlayerSlot random = displayQueue[Random.Range(0, displayQueue.Count)];
        //decided to go in a fixed order for no. less jumping around when players visit makes for a calmer visual experience
        if(availableSlots.Count == 0)
        {
            Debug.LogError("No available slots left");
            return;
        }
        var slot = availableSlots.First();
        availableSlots.Remove(slot.Key);
        occupiedDisplays.Add(faction, slot.Value);
        slot.Value.ActiveFaction = faction;
    }

    public void Hide(PlayerFactions faction)
    {
        if (!occupiedDisplays.ContainsKey(faction))
        {
            Debug.LogWarning("Tried returning Display for Faction that wasn't visiting.");
            return;   // wasn't even visiting
        }
        var slot = occupiedDisplays[faction];
        slot.ActiveFaction = PlayerFactions.None;
        availableSlots.Add(displaySlots.IndexOf(slot), slot);
        occupiedDisplays.Remove(faction);
    }
}
