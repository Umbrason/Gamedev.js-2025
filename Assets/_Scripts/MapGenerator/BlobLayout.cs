using System.Linq;
using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(fileName = "BlobLayout", menuName = "Scriptable Objects/MapGenerator/Layouts/Blob")]
    public class BlobLayout : TilesLayout
    {
        [SerializeField] float scale = 3f;
        [SerializeField] float noiseForce = 0f;

        Vector3 _perlinOffset;
        public override int[] PickPositions(HexPosition[] emptyPositions, int n)
        {
            _perlinOffset = new Vector3(Random.value * 10000f, 0f, Random.value * 10000f);

            return Enumerable.Range(0, emptyPositions.Length)
                .OrderBy(i => EvaluateScalarField(emptyPositions[i].WorldPositionCenter))
                .Take(n)
                .ToArray();
        }

        private float EvaluateScalarField(Vector3 pos)
        {
            Vector3 pos2 = _perlinOffset + pos / scale;
            return Mathf.PerlinNoise(pos2.x, pos2.z) + noiseForce * Random.value;
        }
    }
}
