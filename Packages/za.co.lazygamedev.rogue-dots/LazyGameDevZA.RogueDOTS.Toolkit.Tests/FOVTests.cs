using LazyGameDevZA.RogueDOTS.Toolkit;
using LazyGameDevZA.RogueDOTS.Toolkit.Collections;
using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using static LazyGameDevZA.RogueDOTS.Toolkit.FOV;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Tests
{
    public class FOVTests
    {
        private const int testMapW = 20;
        private const int testMapH = 20;
        private const int testMapTiles = testMapW * testMapH;
        
        private partial struct Map
        {
            public NativeArray<bool> Tiles;
        }

        private partial struct Map
        {
            public static Map New()
            {
                return new Map { Tiles = new NativeArray<bool>(testMapTiles, Allocator.Temp) };
            }
        }

        private partial struct Map : IBaseMap
        {
            public bool IsOpaque(int idx)
            {
                return this.Tiles[idx];
            }
        }
        
        private partial struct Map: IAlgorithm2D
        {
            
            public int point2DToIndex(int2 pt)
            {
                var bounds = this.Dimensions;

                return pt.y * bounds.x + pt.x;
            }

            public int2 Dimensions => new int2(testMapW, testMapH);

            public bool Inbounds(int2 pos)
            {
                var bounds = this.Dimensions;
                return pos.x > 0 && pos.x < bounds.x && pos.y > 0 && pos.y < bounds.y;
            }
        }

        private static bool HasUniqueElements(NativeArray<int2> visible)
        {
            var hashSet = new NativeHashSet<int2>(visible.Length, Allocator.Temp);

            var isUnique = true;
            
            foreach(var t in visible)
            {
                isUnique &= hashSet.TryAdd(t);
            }

            return isUnique;
        }

        [Test]
        public void FOVDupes()
        {
            var map = Map.New();

            var visible = FieldOfView(new int2(10, 10), 8, map);

            Assert.That(HasUniqueElements(visible));
        }

        [Test]
        public void FOVBoundsCheck()
        {
            var map = Map.New();

            var visible = FieldOfView(new int2(2, 2), 8, map);

            foreach(var t in visible)
            {
                Assert.That(t.x > 0);
                Assert.That(t.x < testMapW);
                Assert.That(t.y > 0);
                Assert.That(t.y < testMapH);
            }
        }
    }
}
