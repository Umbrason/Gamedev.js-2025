using UnityEngine;
using UnityEngine.UI;

public class PetitionPhaseHandler : GamePhaseHandler<PetitionPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private PlayerIDButtons VisitButtons;
    [SerializeField] private PetitionResourcePicker resourcePicker;
    [SerializeField] private PetitionBuildingPreview petitionBuildingPreview;
    [SerializeField] private PetitionBuildingPreviewCostDisplay petitionBuildingPreviewCostDisplay;
    [SerializeField] private Button SubmitButton;

    //TODO: Some kind of "Waiting for other players" text after submitting

    private PlayerID m_TargetPlayer = PlayerID.None;
    public PlayerID TargetPlayer
    {
        get => m_TargetPlayer; set
        {
            if (m_TargetPlayer == value) return;
            m_TargetPlayer = value;
            viewer.TargetPlayer = value;
            petitionBuildingPreview.gameObject.SetActive(ActivePetition?.PlayerID == value);
            petitionBuildingPreviewCostDisplay.gameObject.SetActive(ActivePetition?.PlayerID == value);
        }
    }
    public override void OnPhaseEntered()
    {
        SetTargetPlayer(Game.ClientID);
        buildingMenu.OnPlaceBuilding += CreatePetition;
        buildingMenu.CanBuildBuilding += _ => !Phase.ClientPetitionSubmitted;
        VisitButtons.OnClick += SetTargetPlayer;
        resourcePicker.Refresh();
        resourcePicker.OnResourceSourcesModified += petitionBuildingPreviewCostDisplay.Refresh;
        resourcePicker.OnResourceSourcesModified += UpdateSubmitButton;
        SubmitButton.onClick.AddListener(SubmitPetition);
    }

    void UpdateSubmitButton() => SubmitButton.interactable = ActivePetition?.IsFinanced() ?? false;

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
            petitionBuildingPreviewCostDisplay.Petition = value;
            buildingMenu.gameObject.SetActive(m_ActivePetition == null);
            petitionBuildingPreview.gameObject.SetActive(m_ActivePetition?.PlayerID == TargetPlayer);
            petitionBuildingPreviewCostDisplay.gameObject.SetActive(ActivePetition?.PlayerID == TargetPlayer);
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
        buildingMenu.gameObject.SetActive(false);
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
        buildingMenu.gameObject.SetActive(false);
        VisitButtons.gameObject.SetActive(false);
        VisitButtons.OnClick -= SetTargetPlayer;
        resourcePicker.OnResourceSourcesModified -= petitionBuildingPreviewCostDisplay.Refresh;
        resourcePicker.OnResourceSourcesModified -= UpdateSubmitButton;
        SubmitButton.onClick.RemoveListener(SubmitPetition);
    }
}
