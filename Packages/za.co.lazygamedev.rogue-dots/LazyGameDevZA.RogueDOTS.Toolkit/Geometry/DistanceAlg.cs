using System;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Geometry
{
    public enum DistanceAlg
    {
        Pythagoras,
        PythagorasSquared,
        Manhattan,
        Chebyshev,
    }

    public static class DistanceAlgExtensions
    {
        public static float Distance2D(this DistanceAlg self, int2 start, int2 end)
        {
            switch(self)
            {
                case DistanceAlg.Pythagoras:
                    return distance(start, end);
                case DistanceAlg.PythagorasSquared:
                    return distancesq(start, end);
                case DistanceAlg.Manhattan:
                    return csum(abs(start - end));
                case DistanceAlg.Chebyshev:
                    var d = abs(start - end);
                    if(d.x > d.y)
                    {
                        return (d.x - d.y) + 1f * d.y;
                    }
                    else
                    {
                        return (d.y - d.x) + 1f * d.x;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }
        }
    }
}
