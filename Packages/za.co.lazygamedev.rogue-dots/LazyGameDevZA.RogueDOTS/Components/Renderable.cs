using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct Renderable : IComponentData
    {
        public byte Glyph;
        public Colour Foreground;
        public Colour Background;

        #region Nested type: Colour

        public struct Colour
        {
            public byte Red, Green, Blue;

            public Colour(byte red, byte green, byte blue)
            {
                this.Red = red;
                this.Green = green;
                this.Blue = blue;
            }

            public static implicit operator Color(Colour colour)
            {
                var red = colour.Red / 255f;
                var green = colour.Green / 255f;
                var blue = colour.Blue / 255f;
                
                return new Color(red, green, blue);
            }

            public static implicit operator Colour(Color color)
            {
                var red = (byte)math.clamp(color.r * 255, 0f, 255f);
                var green = (byte)math.clamp(color.g * 255, 0f, 255f);
                var blue = (byte)math.clamp(color.b * 255, 0f, 255f);
                
                return new Colour(red, green, blue);
            }
        }

        #endregion
    }
}
