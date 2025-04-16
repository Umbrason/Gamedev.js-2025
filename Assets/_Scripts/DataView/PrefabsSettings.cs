using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

namespace DataView
{
    [CreateAssetMenu(fileName = "TilesPrefabsData", menuName = "Scriptable Objects/TilesPrefabsData")]
    public class PrefabsSettings : ScriptableObject
    {
        [SerializeField] private GameObject[] tilesPrefabs;
        [SerializeField] private GameObject[] buildingsPrefabs;

        private static PrefabsSettings _instance;
        private static PrefabsSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    PrefabsSettings settings = Resources.Load("PrefabsSettings") as PrefabsSettings;
                    if (settings == null)
                    {
                        Debug.LogError("Didn't find a PrefabsSettings.asset file in the Resources folder");
                        return null;
                    }
                    _instance = settings;
                }
                return _instance;
            }
        }


        public static GameObject GetPrefabFor<T>(T value) where T : Enum
        {
            if (Instance == null) return null;

            GameObject[] prefabs = null;

            if (typeof(T) == typeof(Tile)) prefabs = Instance.tilesPrefabs;
            else if(typeof(T) == typeof(Building)) prefabs = Instance.buildingsPrefabs;
            else
            {
                Debug.LogError(string.Format("The enum {0} is not supported", typeof(T)));
                return null;
            }

            int index = Convert.ToInt32(value);

            if (index >= prefabs.Length)
            {
                Debug.LogWarning(string.Format("{2} \"{0}\" does not have an prefab set in {1}", value, Instance.name, typeof(T)));
                return null;
            }

            return prefabs[index];
        }

        public static GameObject GetPrefabFor(Tile tile) => GetPrefabFor<Tile>(tile);
        public static GameObject GetPrefabFor(Building building) => GetPrefabFor<Building>(building);

    }
}
