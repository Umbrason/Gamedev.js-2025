using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDisplay : MonoBehaviour
{
    [SerializeField] private PlayerSlot clientDisplay;
    [SerializeField] private List<PlayerSlot> displaySlots = new List<PlayerSlot>();
    readonly SortedList<int, PlayerSlot> availableSlots = new();
    private readonly Dictionary<PlayerFaction, PlayerSlot> occupiedDisplays = new();

    PlayerFaction m_IslandOwner;
    public PlayerFaction IslandOwner
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

    public PlayerSlot SlotOf(PlayerFaction faction)
    {
        if (IslandOwner == faction) return clientDisplay;
        else return occupiedDisplays.GetValueOrDefault(faction);
    }
    public void Show(PlayerFaction faction, bool isGhost = false)
    {
        if (occupiedDisplays.ContainsKey(faction) || clientDisplay.ActiveFaction == faction)
        {
            return;  // each faction can only be there once
        }
        if (displaySlots.Count <= 0)
        {
            Debug.LogWarning("PlayerDisplay was requested with none left to occupy.");
            return;
        }

        //PlayerSlot random = displayQueue[Random.Range(0, displayQueue.Count)];
        //decided to go in a fixed order for no. less jumping around when players visit makes for a calmer visual experience
        if (availableSlots.Count == 0)
        {
            Debug.LogError("No available slots left");
            return;
        }
        var slot = availableSlots.First();
        availableSlots.Remove(slot.Key);
        occupiedDisplays.Add(faction, slot.Value);
        slot.Value.ActiveFaction = faction;
        slot.Value.IsGhost = isGhost;
    }

    public void Hide(PlayerFaction faction)
    {
        if (!occupiedDisplays.ContainsKey(faction))
            return;   // wasn't even visiting
        var slot = occupiedDisplays[faction];
        slot.ActiveFaction = PlayerFaction.None;
        availableSlots.Add(displaySlots.IndexOf(slot), slot);
        occupiedDisplays.Remove(faction);
    }
}
