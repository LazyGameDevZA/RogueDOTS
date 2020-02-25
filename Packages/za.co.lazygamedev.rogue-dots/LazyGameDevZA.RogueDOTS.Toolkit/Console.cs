using Unity.Entities;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.Toolkit
{
    [InternalBufferCapacity(0)]
    public struct Tile : IBufferElementData
    {
        public byte Glyph;
        public Color32 Foreground;
        public Color32 Background;
    }
    
    public interface IConsole { }
}
