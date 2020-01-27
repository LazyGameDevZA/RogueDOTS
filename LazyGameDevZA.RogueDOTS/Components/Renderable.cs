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
            public float Red, Green, Blue;

            public Colour(byte red, byte green, byte blue)
            {
                this.Red = red / 255f;
                this.Green = green / 255f;
                this.Blue = blue / 255f;
            }

            public Colour(float red, float green, float blue)
            {
                this.Red = red;
                this.Green = green;
                this.Blue = blue;
            }

            public Colour ToGreyscale()
            {
                var linear = this.Red * 0.2126f + this.Green * 0.7152f + this.Blue * 0.0722f;
                return new Colour(linear, linear, linear);
            } 

            public static implicit operator Color(Colour colour) => new Color(colour.Red, colour.Green, colour.Blue);

            public static implicit operator Colour(Color color) => new Colour(color.r,color.g, color.b);
        }

        #endregion
    }
}
