using UnityEngine;

public class PetitionPhaseHandler : GamePhaseHandler<PetitionPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private Canvas VisitButtons;
    [SerializeField] private PetitionResourcePicker resourcePicker;
    [SerializeField] private PetitionBuildingPreview petitionBuildingPreview;

    private PlayerID m_TargetPlayer;
    public PlayerID TargetPlayer
    {
        get => m_TargetPlayer; set
        {
            if (m_TargetPlayer == value) return;
            m_TargetPlayer = value;
            viewer.TargetPlayer = value;
            petitionBuildingPreview.gameObject.SetActive(ActivePetition?.PlayerID == value);
        }
    }
    public override void OnPhaseEntered()
    {
        SetTargetPlayer(Game.ClientID);
        buildingMenu.OnPlaceBuilding += CreatePetition;
        buildingMenu.CanBuildBuilding += _ => Phase.ClientPetitionSubmitted;
        VisitButtons.gameObject.SetActive(true);
    }

    public void UI_SetTargetPlayer(int id) => SetTargetPlayer((PlayerID)id);
    public void SetTargetPlayer(PlayerID player)
    {
        TargetPlayer = player;
    }

    private BuildingPetition m_ActivePetition;
    private BuildingPetition ActivePetition
    {
        get => m_ActivePetition;
        set
        {
            if (value == m_ActivePetition) return;
            m_ActivePetition = value;
            petitionBuildingPreview.Petition = value;
            resourcePicker.ActivePetition = value;
        }
    }
    void CreatePetition(HexPosition position, Building building)
    {
        ActivePetition = new BuildingPetition(TargetPlayer, position, building, new());
    }

    public void CancelPetition()
    {
        ActivePetition = null;
    }

    public void CanSubmitPetition() => ActivePetition.IsFinanced();
    public void SubmitPetition()
    {
        if (ActivePetition == null) return;
        if (!ActivePetition.IsFinanced()) return;
        Phase.SubmitPetition(ActivePetition);
        ActivePetition = null;
    }

    public void SkipPhase()
    {
        Phase.SubmitPetition(null);
        ActivePetition = null;
    }

    public override void OnPhaseExited()
    {
        SetTargetPlayer(PlayerID.None);
        buildingMenu.OnPlaceBuilding -= CreatePetition;
        buildingMenu.CanBuildBuilding = null;
        VisitButtons.gameObject.SetActive(false);
    }
}
