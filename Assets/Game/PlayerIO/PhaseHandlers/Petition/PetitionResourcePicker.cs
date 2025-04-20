using UnityEngine;

public class PetitionResourcePicker : MonoBehaviour
{
    [SerializeField] private PlayerResourceButton PlayerButtonTemplate;
    public BuildingPetition ActivePetition { get; set; }

    void OnEnable()
    {

    }
}
