using System.Collections.Generic;
using UnityEngine;

namespace MapGenerator
{
    public abstract class TilesLayout : ScriptableObject
    {
        public abstract int[] PickPositions(HexPosition[] emptyPositions, int n);
    }
}
