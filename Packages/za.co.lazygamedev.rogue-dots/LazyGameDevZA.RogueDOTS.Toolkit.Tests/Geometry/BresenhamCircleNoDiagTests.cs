using System.Collections.Generic;
using LazyGameDevZA.RogueDOTS.Toolkit.Geometry;
using NUnit.Framework;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Tests.Geometry
{
    public class BresenhamCircleNoDiagTests
    {
        [Test]
        public void CircleNoDiagTestRadius3()
        {
            var circle = new BresenhamCircleNoDiag(int2.zero, 3);
            var enumerator = circle.GetEnumerator();
            var points = new List<int2>(24);

            while(enumerator.MoveNext())
            {
                points.Add(enumerator.Current);
            }

            CollectionAssert.AreEqual(points, new List<int2>
            {
                int2(3, 0),
                int2(0, 3),
                int2(-3, 0),
                int2(0, -3),
                int2(3, 1),
                int2(-1, 3),
                int2(-3, -1),
                int2(1, -3),
                int2(2, 1),
                int2(-1, 2),
                int2(-2, -1),
                int2(1, -2),
                int2(2, 2),
                int2(-2, 2),
                int2(-2, -2),
                int2(2, -2),
                int2(1, 2),
                int2(-2, 1),
                int2(-1, -2),
                int2(2, -1),
                int2(1, 3),
                int2(-3, 1),
                int2(-1, -3),
                int2(3, -1)
            });
        }
    }
}
