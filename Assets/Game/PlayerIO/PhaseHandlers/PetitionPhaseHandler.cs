using UnityEngine;

public class PetitionPhaseHandler : GamePhaseHandler<PetitionPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private Canvas VisitButtons;
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
        viewer.TargetPlayer = player;
    }

    private BuildingPetition ActivePetition;
    void CreatePetition(HexPosition position, Building building)
    {
        PlayerID targetPlayer = viewer.TargetPlayer;
        ActivePetition = new BuildingPetition(targetPlayer, position, building, new());
    }

    public void CancelPetition()
    {
        ActivePetition = null;
    }

    public void SubmitPetition()
    {
        if (ActivePetition == null) return;
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
