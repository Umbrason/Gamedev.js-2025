using UnityEngine;

public class PetitionPhaseHandler : GamePhaseHandler<PetitionPhase>
{
    [SerializeField] private PlayerIslandViewer viewer;
    [SerializeField] private BuildingMenu buildingMenu;
    [SerializeField] private Canvas VisitButtons;
    public override void OnPhaseEntered()
    {
        SetTargetPlayer(Game.ClientID);
        /* buildingMenu.CanBuildBuilding = (building) => !visiting && Phase.CanAffordBuilding(building);
        buildingMenu.OnPlaceBuilding += ; */
        VisitButtons.gameObject.SetActive(true);
    }

    private bool visiting = false;
    public void UI_SetTargetPlayer(int id) => SetTargetPlayer((PlayerID)id);
    public void SetTargetPlayer(PlayerID player)
    {
        visiting = player != Game.ClientID;
        buildingMenu.gameObject.SetActive(!visiting);
        viewer.TargetPlayer = player;
    }   

    void CreatePetition(HexPosition island, Building building)
    {
        PlayerID targetPlayer = viewer.TargetPlayer;
        /* var petition = new BuildingPetition(); */
    }

    void CancelPetition()
    {

    }

    void SubmitPetition()
    {

    }

    public override void OnPhaseExited()
    {
        SetTargetPlayer(PlayerID.None);
        buildingMenu.CanBuildBuilding = null;
        /* buildingMenu.OnPlaceBuilding -= Phase.PlaceBuilding; */
        VisitButtons.gameObject.SetActive(false);
    }
}
