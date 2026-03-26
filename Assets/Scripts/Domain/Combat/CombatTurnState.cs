namespace FlushAndFury.Domain.Combat
{
    public enum TurnOwner
    {
        None = 0,
        Player = 1,
        Enemy = 2,
    }

    public enum CombatTurnPhase
    {
        None = 0,
        BattleStart = 1,
        TurnStart = 2,
        Input = 3,
        Resolve = 4,
        TurnEnd = 5,
        BattleEnd = 6,
    }

    public sealed class CombatTurnState
    {
        public int TurnIndex { get; set; }
        public TurnOwner Owner { get; set; }
        public CombatTurnPhase Phase { get; set; }

        public override string ToString()
        {
            return $"Turn={TurnIndex} Owner={Owner} Phase={Phase}";
        }
    }
}
