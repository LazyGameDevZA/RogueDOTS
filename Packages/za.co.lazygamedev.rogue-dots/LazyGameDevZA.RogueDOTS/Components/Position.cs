using Unity.Entities;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct Position : IComponentData
    {
        public int2 Value;

        public static implicit operator int2(Position position) => position.Value;
        
        public static implicit operator Position(int2 value) => new Position{ Value = value };
    }
}
