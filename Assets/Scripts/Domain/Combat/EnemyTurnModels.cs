namespace FlushAndFury.Domain.Combat
{
    public enum EnemyIntentType
    {
        Attack = 0,
        Defend = 1,
        Debuff = 2,
    }

    public sealed class EnemyIntent
    {
        public EnemyIntentType IntentType { get; set; }
        public int Value { get; set; }
        public string Description { get; set; }
    }

    public sealed class EnemyTurnResult
    {
        public EnemyIntent Intent { get; set; }
        public int DamageToPlayer { get; set; }
        public int BlockGained { get; set; }
        public bool DebuffApplied { get; set; }
    }
}
