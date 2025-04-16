using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(fileName = "TilesBoardGenerator", menuName = "Scriptable Objects/MapGenerator/TilesBoardGenerator")]
    public class TilesBoardGeneratorData : ScriptableObject
    {

        [SerializeField] private TileFrequency[] TilesFrequencies;

        const float SPACING_HORIZONTAL = 1;
        const float SPACING_VERTICAL = 0.866025403784f;

        /// <param name="size">The number of hexagons on each side of the map</param>
        public PlayerIsland Generate(int size = 4, Transform parent = null) => Generate(new HexPosition(0, 0), size, parent);

        /// <param name="size">The number of hexagons on each side of the map</param>
        public PlayerIsland Generate(HexPosition center, int size = 4, Transform parent = null)
        {

            int nTiles = GetBoardNumberOfTiles(size);

            HashSet<HexPosition> emptyPositions = GetTilesPositions().ToHashSet();
            int[] tilesTypesQuantities = PickTilesQuantities();


            Dictionary<HexPosition, Tile> Tiles = new();

            for (int i = 0; i < TilesFrequencies.Length; i++)
            {
                var possiblePositions = emptyPositions.ToArray();
                int[] positionsIndices = TilesFrequencies[i].Layout.PickPositions(possiblePositions, tilesTypesQuantities[i]);

                Tile tile = TilesFrequencies[i].Tile;

                foreach (int index in positionsIndices)
                {
                    HexPosition pos = possiblePositions[index];

                    emptyPositions.Remove(pos);

                    Tiles.Add(pos, tile);
                }
            }

            return new PlayerIsland(Tiles);

            HexPosition[] GetTilesPositions()
            {
                HexPosition[] positions = new HexPosition[nTiles];

                int i = 0;

                positions[i++] = center;

                HexPosition[] radialDirections = {
                    new HexPosition(1,-1,0),
                    new HexPosition(1,0,-1),
                    new HexPosition(0,1,-1),
                    new HexPosition(-1,1,0),
                    new HexPosition(-1,0,1),
                    new HexPosition(0,-1,1)
                };

                HexPosition[] orbitalDirections = new HexPosition[6];
                for (int d = 0; d <= 5; d++)
                    orbitalDirections[d] = radialDirections[(d + 1) % 6] - radialDirections[d];

                for (int radius = 1; radius < size; radius++)
                {
                    for(int direction = 0; direction < 6; direction++)
                    {
                        for(int j = 0; j < radius; j++)
                        {
                            positions[i++] = center + radius * radialDirections[direction] + j * orbitalDirections[direction];
                        }
                    }
                }

                return positions;
            }

            int[] PickTilesQuantities()
            {
                float[] frequencies = new float[TilesFrequencies.Length];

                bool nullTotFreq = true;
                for (int i = 0; i < TilesFrequencies.Length; i++)
                {
                    TileFrequency layout = TilesFrequencies[i];
                    float frequency = UnityEngine.Random.Range(layout.MinFrequency, layout.MaxFrequency);
                    if (frequency < 0) frequency = 0;
                    frequencies[i] = frequency;
                    if (frequency > 0) nullTotFreq = false;
                }

                if (nullTotFreq)
                {
                    Debug.LogError("All tiles frequencies are set to 0");
                    return new int[TilesFrequencies.Length];
                }

                return AllocateWithLargestRemainder(frequencies, nTiles);
            }
            
        }

        private static int GetBoardNumberOfTiles(int size)
        {
            if(size <= 0) return 0;

            int n = 1;
            for (int i = 1; i < size; i++)
            {
                n += 6 * i;
            }
            return n;
        }

        public static int[] AllocateWithLargestRemainder(float[] frequencies, int total)
        {
            int n = frequencies.Length;
            float totalFreq = frequencies.Sum();

            float[] ideal = new float[n];
            int[] floorAlloc = new int[n];
            float[] remainders = new float[n];

            for (int i = 0; i < n; i++)
            {
                ideal[i] = frequencies[i] / totalFreq * total;
                floorAlloc[i] = (int)Math.Floor(ideal[i]);
                remainders[i] = ideal[i] - floorAlloc[i];
            }

            int remaining = total - floorAlloc.Sum();

            var sortedIndices = Enumerable.Range(0, n)
                                          .OrderByDescending(i => remainders[i])
                                          .ToArray();

            for (int i = 0; i < remaining; i++)
            {
                floorAlloc[sortedIndices[i]] += 1;
            }

            return floorAlloc;
        }

        public enum TileLayoutType { Uniform, Blob, Line }

        [System.Serializable]
        public struct TileFrequency
        {
            public Tile Tile;
            public float MinFrequency;
            public float MaxFrequency;
            public TilesLayout Layout;
        }
    }
}

