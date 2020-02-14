using Unity.Entities;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct VisibleTilePosition : IBufferElementData
    {
        public int2 Value;
    }

    public struct ViewshedData: IComponentData
    {
        public int Range;
    }
}
