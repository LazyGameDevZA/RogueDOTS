using Unity.Entities;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct PlayerPosition : IComponentData
    {
        public Entity Entity;
        
        public int2 Value;
        
        public static implicit operator int2(PlayerPosition playerPosition) => playerPosition.Value;
    }
}
