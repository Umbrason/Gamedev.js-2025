using System.Collections.Generic;
using UnityEngine;

namespace DataView
{
    [CreateAssetMenu(fileName = "BuildingsPrefabsData", menuName = "Scriptable Objects/BuildingsPrefabsData")]
    public class BuildingsPrefabsData : ScriptableObject
    {
        [SerializeField] private GameObject[] buildingsPrefabs;

        public GameObject GetBuildingPrefab(Building building)
        {
            int index = (int)building;

            if(index >= buildingsPrefabs.Length)
            {
                Debug.LogWarning(string.Format("Building \"{0}\" does not have an prefab set in {1}", building, name));
                return null;
            }

            return buildingsPrefabs[index];
        }    
    }
}
