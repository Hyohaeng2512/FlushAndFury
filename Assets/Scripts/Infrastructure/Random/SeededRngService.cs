using SystemRandom = System.Random;

namespace FlushAndFury.Infrastructure.Random
{
    public sealed class SeededRngService : IRngService
    {
        private readonly SystemRandom random;

        public SeededRngService(int seed)
        {
            random = new SystemRandom(seed);
        }

        public int NextInt(int minInclusive, int maxExclusive)
        {
            return random.Next(minInclusive, maxExclusive);
        }

        public float NextFloat()
        {
            return (float)random.NextDouble();
        }
    }
}
