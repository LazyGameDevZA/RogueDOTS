using LazyGameDevZA.RogueDOTS.Toolkit.Collections;
using LazyGameDevZA.RogueDOTS.Toolkit.Geometry;
using Unity.Collections;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Toolkit
{
    public static class FOV
    {
        // TODO Map might be switched out for an interface if possible
        public static NativeHashSet<int2> FieldOfViewSet<T>(int2 start, int range, in T fovCheck) where T: IAlgorithm2D, IBaseMap
        {
            var visiblePoints = new NativeHashSet<int2>( range * 2 * range * 2, Allocator.Temp);

            var iterator = new BresenhamCircleNoDiag(start, range).GetEnumerator();
            
            while(iterator.MoveNext())
            {
                ScanFovLine(start, iterator.Current, fovCheck, ref visiblePoints);
            }
            
            iterator.Dispose();

            return visiblePoints;
        }
        
        public static NativeArray<int2> FieldOfView<T>(int2 start, int range, in T fovCheck) where T: IAlgorithm2D, IBaseMap
        {
            return FieldOfViewSet(start, range, fovCheck).ToNativeArray();
        }

        private static void ScanFovLine<T>(int2 start, int2 end, in T fovCheck, ref NativeHashSet<int2> visiblePoints) where T : IAlgorithm2D, IBaseMap
        {
            var line = VectorLine.New(start, end);

            var iterator = line.GetEnumerator();
            int2 target;
            
            while(iterator.MoveNext())
            {
                target = iterator.Current;
                if(!fovCheck.Inbounds(target))
                {
                    // We're outside the map
                    break;
                }

                visiblePoints.TryAdd(target);

                if(fovCheck.IsOpaque(fovCheck.point2DToIndex(target)))
                {
                    //FOV is blocked
                    break;
                }
            }
            
            iterator.Dispose();
        }
    }
}
