using LazyGameDevZA.RogueDOTS.Toolkit.Collections;
using NUnit.Framework;
using Unity.Collections;
using static LazyGameDevZA.RogueDOTS.Toolkit.FOV;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

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
        
        private static int xy_idx(int x, int y)
        {
            return y * testMapW + x;
        }

        private static int2 positionFromText(string template)
        {
            var y = 0;
            foreach (var line in template.Split('\n'))
            {
                var x = 0;
                foreach (var ch in line.ToCharArray())
                {
                    if (ch == '@')
                    {
                        return int2(x, y);
                    }

                    x++;
                }

                y++;
            }

            return int2.zero;
        }

        private partial struct Map
        {
            public static Map New()
            {
                return new Map { Tiles = new NativeArray<bool>(testMapTiles, Allocator.Temp) };
            }

            public static Map FromText(string template)
            {
                var result = New();
                var y = 0;

                foreach (var line in template.Split('\n'))
                {
                    var x = 0;
                    foreach (var ch in line.ToCharArray())
                    {
                        switch (ch)
                        {
                            case '#':
                            case ' ':
                                result.Tiles[xy_idx(x, y)] = true;
                                break;
                            default:
                                result.Tiles[xy_idx(x, y)] = false;
                                break;
                        }
                         
                        x++;
                    }

                    y++;
                }

                return result;
            }
        }

        private partial struct Map : IBaseMap
        {
            public bool IsOpaque(int idx)
            {
                return this.Tiles[idx];
            }

            public NativeList<Exit> GetAvailableExits(int idx, NativeList<Exit> exits = default)
            {
                throw new System.NotImplementedException();
            }

            public float GetPathingDistance(int idx1, int idx2)
            {
                throw new System.NotImplementedException();
            }
        }
        
        private partial struct Map: IAlgorithm2D
        {
            
            public int point2DToIndex(int2 pt)
            {
                var bounds = this.Dimensions;

                return pt.y * bounds.x + pt.x;
            }

            public int2 Dimensions => int2(testMapW, testMapH);

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

            var visible = FieldOfView(int2(10, 10), 8, map);

            Assert.That(HasUniqueElements(visible));
        }

        [Test]
        public void FOVBoundsCheck()
        {
            var map = Map.New();

            var visible = FieldOfView(int2(2, 2), 8, map);

            foreach(var t in visible)
            {
                Assert.That(t.x > 0);
                Assert.That(t.x < testMapW);
                Assert.That(t.y > 0);
                Assert.That(t.y < testMapH);
            }
        }

        [Test]
        public void FOVHasNoDeadSpots()
        {
            /*
             * This is not a very good test... it was arrived at by manually testing behaviour in Unity until it
             * was acceptable and then pegging the number of visible tiles in the test scenario. It was good enough
             * to at least detect when my refactor broke the current behaviour, so leaving it in for now.
             *
             * It would be better if:
             *  - there was a nicer way to load the test scenario
             *  - a way to visualise the output of the FOV code in test output
             */
            var testMap = 
@"
       .      
       .      
       .      
      #.#
      #.#      
      #.#
   ####.#####
   #........#
   #...@....#
   #........#
   #........#
   #........#
 ...............
####........#...
   ##########
";
            var position = positionFromText(testMap);
            var range = 8;
            var map = Map.FromText(testMap);
            
            var visible = FieldOfView(position, range, map);

            Assert.AreEqual(108, visible.Length);
        }
    }
}
