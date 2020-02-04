using System.Collections.Generic;
using LazyGameDevZA.RogueDOTS.Toolkit.Geometry;
using NUnit.Framework;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Tests.Geometry
{
    public class BresenhamCircleNoDiagTest
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
                new int2(3, 0),
                new int2(0, 3),
                new int2(-3, 0),
                new int2(0, -3),
                new int2(3, 1),
                new int2(-1, 3),
                new int2(-3, -1),
                new int2(1, -3),
                new int2(2, 1),
                new int2(-1, 2),
                new int2(-2, -1),
                new int2(1, -2),
                new int2(2, 2),
                new int2(-2, 2),
                new int2(-2, -2),
                new int2(2, -2),
                new int2(1, 2),
                new int2(-2, 1),
                new int2(-1, -2),
                new int2(2, -1),
                new int2(1, 3),
                new int2(-3, 1),
                new int2(-1, -3),
                new int2(3, -1)
            });
        }
    }
}
