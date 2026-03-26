namespace FlushAndFury.Domain.Combat
{
    public sealed class BoonContext
    {
        public int FlatDamageBonus { get; set; }
        public float DamageMultiplier { get; set; } = 1f;
    }
}
