using UnityEngine;

public class PetitionBuildingPreview : MonoBehaviour
{
    [SerializeField] private BuildingView buildingView;
    private BuildingPetition m_Petition;
    public BuildingPetition Petition
    {
        get => m_Petition;
        set
        {
            if (m_Petition == value) return;
            m_Petition = value;
            buildingView.Data = m_Petition.Building;
            transform.position = HexOrientation.Active * m_Petition.Position;
        }
    }
}