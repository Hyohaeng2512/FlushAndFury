namespace FlushAndFury.Domain.Combat
{
    public sealed class CardBuffContext
    {
        public int FlatDamageBonus { get; set; }
        public float DamageMultiplier { get; set; } = 1f;
    }
}
