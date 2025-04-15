using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public class MapGeneratorSample : MonoBehaviour
    {
        [SerializeField] Vector2Int mapQuantities = new Vector2Int(8,5);
        [SerializeField] Vector2 distance = new Vector2(8,8);
        [SerializeField] int mapSize = 4;

        [SerializeField] TilesBoardGeneratorData[] Generators;

        private void Start()
        {
            Generate();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Generate();
        }

        private void Generate()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            for(int X = 0; X < mapQuantities.x; X++)
            {
                for (int Z = 0; Z < mapQuantities.y; Z++)
                {
                    float x = (X - 0.5f * (mapQuantities.x - 1f)) * distance.x;
                    float z = (Z - 0.5f * (mapQuantities.y - 1f)) * distance.y;

                    Generators[i++ % Generators.Length].Generate(new Vector3(x, 0f, z), mapSize, transform);
                }
            }
        }
    }
}
