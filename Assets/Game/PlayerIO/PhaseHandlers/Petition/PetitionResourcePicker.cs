using System.Collections.Generic;
using UnityEngine;

public class PetitionResourcePicker : MonoBehaviour
{
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

    void Start()
    {
        for (PlayerID playerID = 0; (int)playerID < 6; playerID++)
        {
            buttonInstances[playerID] = Instantiate(PlayerButtonTemplate);
            var pID = playerID;
            void OnResourceCounterChanged(Resource resource, int amount)
            {
                ActivePetition.ResourceSources[pID][resource] = amount;
                foreach (var instance in buttonInstances)
                {
                    if (instance.Key == pID) continue;
                    instance.Value.ResourceInputInstances[resource].Max = ActivePetition.Building.ConstructionCosts()[resource] - ActivePetition.ResourceSources[pID][resource];
                }
            }
            buttonInstances[pID].OnResourceCounterChanged += OnResourceCounterChanged;
        }
    }
}
