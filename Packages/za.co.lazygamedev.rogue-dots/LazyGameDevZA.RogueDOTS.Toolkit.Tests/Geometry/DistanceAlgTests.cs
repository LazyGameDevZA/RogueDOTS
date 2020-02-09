using LazyGameDevZA.RogueDOTS.Toolkit.Geometry;
using NUnit.Framework;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Tests.Geometry
{
    public class DistanceAlgTests
    {
        [Test]
        public void TestPythagorasDistance()
        {
            var d = DistanceAlg.Pythagoras.Distance2D(int2.zero, int2(5, 0));
            Assert.That(abs(d - 5.0f) < float.Epsilon);

            d = DistanceAlg.Pythagoras.Distance2D(int2.zero,int2(-5, 0));
            Assert.That(abs(d - 5.0f) < float.Epsilon);

            d = DistanceAlg.Pythagoras.Distance2D(int2.zero, int2(0, 5));
            Assert.That(abs(d - 5.0f) < float.Epsilon);

            d = DistanceAlg.Pythagoras.Distance2D(int2.zero, int2(0, -5));
            Assert.That(abs(d - 5.0f) < float.Epsilon);

            d = DistanceAlg.Pythagoras.Distance2D(int2(0, +2), int2(0, -3));
            Assert.That(abs(d - 5.0f) < float.Epsilon);
            
            d = DistanceAlg.Pythagoras.Distance2D(int2.zero, int2(5, 5));
            Assert.That(abs(d - 7.071_068f) < float.Epsilon);
        }

        [Test]
        public void TestPythagorasSquaredDistance()
        {
            var d = DistanceAlg.PythagorasSquared.Distance2D(int2.zero, int2(5, 0));
            Assert.That(abs(d - 25.0f) < float.Epsilon);

            d = DistanceAlg.PythagorasSquared.Distance2D(int2.zero,int2(-5, 0));
            Assert.That(abs(d - 25.0f) < float.Epsilon);

            d = DistanceAlg.PythagorasSquared.Distance2D(int2.zero, int2(0, 5));
            Assert.That(abs(d - 25.0f) < float.Epsilon);

            d = DistanceAlg.PythagorasSquared.Distance2D(int2.zero, int2(0, -5));
            Assert.That(abs(d - 25.0f) < float.Epsilon);

            d = DistanceAlg.PythagorasSquared.Distance2D(int2.zero, int2(5, 5));
            var f = abs(d - 50.0f);
            Assert.That(f < float.Epsilon);
        }
        
        [Test]
        public void TestManhattanDistance() {
            var d = DistanceAlg.Manhattan.Distance2D(int2.zero, int2(5, 0));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Manhattan.Distance2D(int2.zero, int2(-5, 0));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Manhattan.Distance2D(int2.zero, int2(0, 5));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Manhattan.Distance2D(int2.zero, int2(0, -5));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Manhattan.Distance2D(int2.zero, int2(5, 5));
            Assert.That(abs(d - 10.0) < float.Epsilon);
        }
        
        [Test]
        public void TestChebyshevDistance() {
            var d = DistanceAlg.Chebyshev.Distance2D(int2.zero, int2(5, 0));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Chebyshev.Distance2D(int2.zero, int2(-5, 0));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Chebyshev.Distance2D(int2.zero, int2(0, 5));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Chebyshev.Distance2D(int2.zero, int2(0, -5));
            Assert.That(abs(d - 5.0) < float.Epsilon);

            d = DistanceAlg.Chebyshev.Distance2D(int2.zero, int2(5, 5));
            Assert.That(abs(d - 5.0) < float.Epsilon);
        }
    }
}
