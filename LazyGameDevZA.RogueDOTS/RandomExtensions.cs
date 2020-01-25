using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS
{
    public static class RandomExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RollDice(ref this Random random, int min, int max)
        {
            return random.NextInt(min, max + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Range(ref this Random random, int min, int max)
        {
            return random.NextInt(min, max + 1);
        }
    }
}
