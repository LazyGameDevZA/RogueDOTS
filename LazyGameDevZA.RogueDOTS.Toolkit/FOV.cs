using LazyGameDevZA.RogueDOTS.Toolkit.Collections;
using LazyGameDevZA.RogueDOTS.Toolkit.Geometry;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS.Toolkit
{
    public static class FOV
    {
        // TODO Map might be switched out for an interface if possible
        public static NativeHashSet<int2> FieldOfViewSet<T>(int2 start, int range, in T fovCheck)
            where T : IAlgorithm2D, IBaseMap
        {
            var visiblePoints = new NativeHashSet<int2>(range * 2 * range * 2, Allocator.Temp);

            var iterator = new BresenhamCircleNoDiag(start, range).GetEnumerator();
            
            while(iterator.MoveNext())
            {
                ScanFovLine(start, iterator.Current, fovCheck, ref visiblePoints);
            }

            iterator.Dispose();

            PostProcessToRemoveBresenhamArtifacts(start, range, fovCheck, visiblePoints);

            return visiblePoints;
        }

        private static void PostProcessToRemoveBresenhamArtifacts<T>(int2 start, int range, in T fovCheck,
            NativeHashSet<int2> visiblePoints) where T : IAlgorithm2D, IBaseMap
        {
            RemoveBresenhamArtifactsInDirection(start, int2(-1, -1), range, fovCheck, visiblePoints);
            RemoveBresenhamArtifactsInDirection(start, int2(+1, -1), range, fovCheck, visiblePoints);
            RemoveBresenhamArtifactsInDirection(start, int2(-1, +1), range, fovCheck, visiblePoints);
            RemoveBresenhamArtifactsInDirection(start, int2(+1, +1), range, fovCheck, visiblePoints);
        }

        private static void RemoveBresenhamArtifactsInDirection<T>(
            int2 start,
            int2 direction,
            int range,
            in T fovCheck,
            NativeHashSet<int2> visiblePoints) where T : IAlgorithm2D, IBaseMap
        {
            for (var i = 0; i <= range + 1; i++)
            {
                var x = start.x + i * direction.x;
                for (var j = 0; j <= range + 1; j++)
                {
                    var y = start.y + j * direction.y;
                    
                    var current = int2(x, y);
                    var isWall = fovCheck.Inbounds(current) && fovCheck.IsOpaque(fovCheck.point2DToIndex(current));
                    var horizontalInFront = int2(x - direction.x, y);
                    var verticalInFront = int2(x, y - direction.y);
                    var horizontalInFrontIsLitGround = fovCheck.Inbounds(horizontalInFront) && 
                                          !fovCheck.IsOpaque(fovCheck.point2DToIndex(horizontalInFront)) &&
                                          visiblePoints.Contains(horizontalInFront);
                    var verticalInFrontIsLitGround = fovCheck.Inbounds(verticalInFront) &&
                                           !fovCheck.IsOpaque(fovCheck.point2DToIndex(verticalInFront)) &&
                                           visiblePoints.Contains(verticalInFront);

                    if (isWall && (verticalInFrontIsLitGround || horizontalInFrontIsLitGround))
                    {
                        visiblePoints.TryAdd(current);
                    }
                }
            }
        }

        public static NativeArray<int2> FieldOfView<T>(int2 start, int range, in T fovCheck)
            where T : IAlgorithm2D, IBaseMap
        {
            return FieldOfViewSet(start, range, fovCheck).ToNativeArray();
        }

        private static void ScanFovLine<T>(int2 start, int2 end, in T fovCheck, ref NativeHashSet<int2> visiblePoints)
            where
            T : IAlgorithm2D, IBaseMap
        {
            var line = VectorLine.New(start, end);
            var iterator = line.GetEnumerator();

            int2 target;
            while (iterator.MoveNext())
            {
                target = iterator.Current;
                if (!fovCheck.Inbounds(target))
                {
                    // We're outside the map
                    break;
                }

                visiblePoints.TryAdd(target);

                if (fovCheck.IsOpaque(fovCheck.point2DToIndex(target)))
                {
                    //FOV is blocked
                    break;
                }
            }

            iterator.Dispose();
        }
    }
}