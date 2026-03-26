namespace FlushAndFury.Domain.Combat
{
    public enum CombatSide
    {
        Player = 0,
        Enemy = 1,
    }

    public enum CombatStatusType
    {
        Strength = 0,
        Burn = 1,
        Vulnerable = 2,
        Weak = 3,
    }

    public sealed class CombatStatusInstance
    {
        public CombatStatusType Type { get; set; }
        public int Stacks { get; set; }
        public int RemainingTurns { get; set; }
        public bool IsPermanent { get; set; }
    }
}
