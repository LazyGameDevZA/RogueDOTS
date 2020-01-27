using Unity.Mathematics;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.TerminalRenderer
{
    public static class int2Conversion
    {
        public static Vector3Int AsVector3Int(this int2 v)     { return new Vector3Int(v.x, v.y, 0); }
    }
}
