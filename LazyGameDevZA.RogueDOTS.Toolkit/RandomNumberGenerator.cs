using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Toolkit
{
    public partial struct RandomNumberGenerator
    {
        private Random random;
    }

    public partial struct RandomNumberGenerator
    {
        public static RandomNumberGenerator New()
        {
            IntUnionUInt union = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

            return new RandomNumberGenerator { random = new Random(union) };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int RollDice(int n, int dieType)
        {
            var total = 0;

            for(int i = 0; i < n; i++)
            {
                total += this.Range(1, dieType + 1);
            }

            return total;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int min, int max)
        {
            return random.NextInt(min, max);
        }

        #region Nested type: IntUnionUInt

        [StructLayout(LayoutKind.Explicit)]
        private struct IntUnionUInt
        {
            [FieldOffset(0)]
            private int intValue;

            [FieldOffset(0)]
            private readonly uint uintValue;

            public static implicit operator IntUnionUInt(int value) => new IntUnionUInt { intValue = value };

            public static implicit operator uint(IntUnionUInt union) => union.uintValue;
        }

        #endregion
    }
}
