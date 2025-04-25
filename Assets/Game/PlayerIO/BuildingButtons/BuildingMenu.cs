using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] GameInstance instance;
    [SerializeField] BuildingButton buttonTemplate;
    private readonly List<BuildingButton> buttonInstances = new();
    [SerializeField] Transform container;

    public event Action<HexPosition, Building> OnPlaceBuilding;
    public Func<Building, bool> CanBuildBuilding;

    [SerializeField] GhostBuildingView BuildingPreview;

    void Start()
    {
        var buildings = (Building[])Enum.GetValues(typeof(Building));
        foreach (var building in buildings)
        {
            if (building == Building.None) continue;
            var button = Instantiate(buttonTemplate, container ?? transform);
            button.Building = building;
            buttonInstances.Add(button);

            void OnDrop(HexPosition pos) => OnPlaceBuilding?.Invoke(pos, button.Building);
            void OnStartDrag(Building b) => BuildingPreview.Data = b;
            void OnStopDrag() => BuildingPreview.Data = Building.None;
            void OnUpdateDrag(Vector3 p) => BuildingPreview.transform.position = p;
            button.OnDrop += OnDrop;
            button.OnStartDrag += OnStartDrag;
            button.OnStopDrag += OnStopDrag;
            button.OnUpdateDrag += OnUpdateDrag;

            button.Active = CanBuildBuilding?.Invoke(button.Building) ?? true;
        }
    }

    void Update()
    {
        foreach (var button in buttonInstances)
            button.Active = CanBuildBuilding?.Invoke(button.Building) ?? true;
    }
}
