using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS.Components
{
    [InternalBufferCapacity(0)]
    public struct Tile: IBufferElementData
    {

        public TileType Type;

        public static implicit operator Tile(TileType type) => new Tile { Type = type};
    }
    
    public enum TileType
    {
        Floor, Wall
    }
}
