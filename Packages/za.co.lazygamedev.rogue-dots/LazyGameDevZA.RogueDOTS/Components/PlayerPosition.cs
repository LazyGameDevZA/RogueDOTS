using Unity.Entities;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct PlayerPosition: IComponentData
    {
        public int2 Value;
        
        public static implicit operator PlayerPosition(int2 value) => new PlayerPosition{ Value = value };

        public static implicit operator int2(PlayerPosition playerPosition) => playerPosition.Value;
    }
}
