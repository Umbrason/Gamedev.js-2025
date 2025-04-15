using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public abstract class TilesLayout : ScriptableObject
    {
        public abstract int[] PickPositions(Vector3[] emptyPositions, int n);
    }
}
