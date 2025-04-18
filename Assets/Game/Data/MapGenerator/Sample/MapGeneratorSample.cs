using DataView;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MapGenerator
{
    public class MapGeneratorSample : MonoBehaviour
    {
        [SerializeField] Vector2Int mapQuantities = new Vector2Int(8, 5);
        [SerializeField] Vector2 distance = new Vector2(8, 8);
        [SerializeField] int mapSize = 4;

        [SerializeField] TilesBoardGeneratorData[] Generators;
        [SerializeField] IslandView IslandViewPrefab;

        private void Start()
        {
            Generate();
        }

        private void Update()
        {
            if (Keyboard.current[Key.Space].wasPressedThisFrame) Generate();
        }

        private void Generate()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            for (int X = 0; X < mapQuantities.x; X++)
            {
                for (int Z = 0; Z < mapQuantities.y; Z++)
                {
                    float x = (X - 0.5f * (mapQuantities.x - 1f)) * distance.x;
                    float z = (Z - 0.5f * (mapQuantities.y - 1f)) * distance.y;

                    Vector3 pos = new Vector3(x, 0f, z);
                    // PlayerIsland island = Generators[i++ % Generators.Length].Generate(HexOrientation.Active * pos, mapSize, transform);

                    var IslandInstance = Instantiate(IslandViewPrefab, pos, Quaternion.identity, transform);

                    var playerIsland = Generators[i++ % Generators.Length].Generate(mapSize);

                    HashSet<HexPosition> hexPositions = playerIsland.Tiles.Keys.ToHashSet();
                    List<(HexPosition, Building)> buildings = new();
                    foreach (HexPosition p in hexPositions)
                    {
                        if (Random.value > 0.95f) buildings.Add((p, Building.Mine));
                    }

                    playerIsland = playerIsland.WithBuildings(buildings.ToArray());

                    IslandInstance.Data = playerIsland;
                }
            }
        }
    }
}
