using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOverview : MonoBehaviour
{
    [SerializeField] private TMP_Text buildingName;
    [SerializeField] private Image icon;    // do we want this?
    [SerializeField] private TMP_Text description;

    public void ChangeBuilding(Building building, Sprite img)
    {
        buildingName.text = building.ToString();
        icon.sprite = img;
        // description.text = building.Description;
    }
}
