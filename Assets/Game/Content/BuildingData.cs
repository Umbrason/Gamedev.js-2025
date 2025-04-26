using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building")]
public class BuildingData : ScriptableObject
{
    public Building Building;
    public ResourcesQuantities Cost;
    public ResourcesQuantities OperationCost;
    public string Description;
}
