using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS
{
    public static class RandomExtensions
    {
        public static int RollDice(ref this Random random, int min, int max)
        {
            return random.NextInt(min, max + 1);
        }
    }
}
