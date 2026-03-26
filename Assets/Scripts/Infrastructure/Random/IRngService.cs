namespace FlushAndFury.Infrastructure.Random
{
    public interface IRngService
    {
        int NextInt(int minInclusive, int maxExclusive);
        float NextFloat();
    }
}
