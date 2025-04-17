using System.Linq;
using UnityEngine;

namespace MapGenerator
{
    [CreateAssetMenu(fileName = "RandomLayout", menuName = "Scriptable Objects/MapGenerator/Layouts/Random")]
    public class RandomLayout : TilesLayout
    {
        public override int[] PickPositions(HexPosition[] emptyPositions, int n)
        {
            return Enumerable.Range(0, emptyPositions.Length)
                .OrderBy(x => Random.value)
                .Take(n)
                .ToArray();
        }
    }
}
