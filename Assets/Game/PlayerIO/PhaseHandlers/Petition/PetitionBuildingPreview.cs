using UnityEngine;

public class PetitionBuildingPreview : MonoBehaviour
{
    [SerializeField] private GhostBuildingView buildingView;
    private BuildingPetition m_Petition;
    public BuildingPetition Petition
    {
        get => m_Petition;
        set
        {
            if (m_Petition == value) return;
            m_Petition = value;
            buildingView.Data = m_Petition?.Building ?? Building.None;
            if (value == null) return;
            transform.position = m_Petition.Position.WorldPositionCenter;
        }
    }
}