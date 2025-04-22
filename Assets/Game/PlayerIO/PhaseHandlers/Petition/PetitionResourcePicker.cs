using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PetitionResourcePicker : MonoBehaviour
{
    [SerializeField] private PetitionPhaseHandler petitionPhaseHandler;
    [SerializeField] private PlayerResourceButton PlayerButtonTemplate;
    private readonly Dictionary<PlayerID, PlayerResourceButton> buttonInstances = new();

    BuildingPetition m_ActivePetition;
    public BuildingPetition ActivePetition
    {
        get => m_ActivePetition;
        set
        {
            m_ActivePetition = value;
            UIRoot.SetActive(value != null);
            foreach (var instance in buttonInstances.Values)
                instance.ActivePetition = value;
        }
    }
    [SerializeField] GameObject UIRoot;
    [SerializeField] Transform buttonContainer;

    public event Action OnResourceSourcesModified;

    public void Refresh()
    {
        Clear();
        for (PlayerID playerID = 0; (int)playerID < 6; playerID++)
        {
            buttonInstances[playerID] = Instantiate(PlayerButtonTemplate, buttonContainer);
            buttonInstances[playerID].PlayerFaction = petitionPhaseHandler.Game.PlayerData[playerID].Faction;
            buttonInstances[playerID].TargetPlayer = playerID;
            var pID = playerID;
            void OnResourceCounterChanged(Resource resource, int amount)
            {
                if (!ActivePetition.ResourceSources.ContainsKey(pID))
                    ActivePetition.ResourceSources[pID] = new();
                if (amount > 0) ActivePetition.ResourceSources[pID][resource] = amount;
                else ActivePetition.ResourceSources[pID].Remove(resource);
                foreach (var instance in buttonInstances)
                {
                    if (instance.Key == pID) continue;
                    var otherPlayersContributions = ActivePetition.ResourceSources.Where(p => p.Key != instance.Value.TargetPlayer);
                    var amountCoveredByOtherPlayers = otherPlayersContributions.Sum(p => p.Value?.GetValueOrDefault(resource) ?? 0);
                    var limit = ActivePetition.Building.ConstructionCosts()[resource] - amountCoveredByOtherPlayers;
                    instance.Value.ResourceInputInstances[resource].Max = limit;
                }
                OnResourceSourcesModified?.Invoke();
            }
            buttonInstances[pID].OnResourceCounterChanged += OnResourceCounterChanged;
        }
    }

    public void Clear()
    {
        foreach (var instance in buttonInstances.Values)
        {
            Destroy(instance.gameObject);
        }
        buttonInstances.Clear();
    }
}
