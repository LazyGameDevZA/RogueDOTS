using Unity.Entities;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct Move: IComponentData
    {
        public int2 Value;
        
        public static implicit operator Move(int2 value) => new Move { Value = value };

        public static implicit operator int2(Move move) => move.Value;
    }
}
