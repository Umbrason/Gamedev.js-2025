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

    void Start()
    {
        var buildings = (Building[])Enum.GetValues(typeof(Building));
        foreach (var building in buildings)
        {
            var button = Instantiate(buttonTemplate, container ?? transform);
            button.Building = building;
            buttonInstances.Add(button);
            button.OnDrop += (pos) => OnPlaceBuilding?.Invoke(pos, button.Building);
            button.Active = CanBuildBuilding?.Invoke(button.Building) ?? true;
        }
    }

    void Update()
    {
        foreach (var button in buttonInstances)
            button.Active = CanBuildBuilding?.Invoke(button.Building) ?? true;
    }
}
