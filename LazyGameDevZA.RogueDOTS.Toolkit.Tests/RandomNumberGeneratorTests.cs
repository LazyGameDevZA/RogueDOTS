using System.Linq;
using NUnit.Framework;

namespace LazyGameDevZA.RogueDOTS.Toolkit.Tests
{
    public class RandomNumberGeneratorTests
    {
        [Test]
        public void TestRollRange()
        {
            var rng = RandomNumberGenerator.New();

            foreach(var _ in Enumerable.Range(0, 100))
            {
                var n = rng.RollDice(1, 20);
                Assert.That(n > 0 && n < 21);
            }
        }
    }
}
