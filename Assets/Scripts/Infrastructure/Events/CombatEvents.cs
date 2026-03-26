namespace FlushAndFury.Infrastructure.Events
{
    public readonly struct EnergyGainRequested
    {
        public readonly int Amount;

        public EnergyGainRequested(int amount)
        {
            Amount = amount;
        }
    }

    public readonly struct ApplyStatusToTargetRequested
    {
        public readonly string StatusId;
        public readonly int Value;

        public ApplyStatusToTargetRequested(string statusId, int value)
        {
            StatusId = statusId;
            Value = value;
        }
    }

    public readonly struct LuckyProcCheckRequested
    {
    }

    public readonly struct CardReturnToDeckRequested
    {
    }

    public readonly struct GoldOnKillFlagRequested
    {
    }

    public readonly struct CardPlaceTopDeckRequested
    {
    }

    public readonly struct ApplyStatusToPlayerRequested
    {
        public readonly string StatusId;
        public readonly int Value;
        public readonly int DurationTurns;

        public ApplyStatusToPlayerRequested(string statusId, int value, int durationTurns)
        {
            StatusId = statusId;
            Value = value;
            DurationTurns = durationTurns;
        }
    }

    public readonly struct DealRandomEnemyDamageRequested
    {
        public readonly int Amount;

        public DealRandomEnemyDamageRequested(int amount)
        {
            Amount = amount;
        }
    }

    public readonly struct DrawCardRequested
    {
        public readonly int Amount;

        public DrawCardRequested(int amount)
        {
            Amount = amount;
        }
    }

    public readonly struct ReflectDamageToPlayerRequested
    {
        public readonly int Amount;

        public ReflectDamageToPlayerRequested(int amount)
        {
            Amount = amount;
        }
    }

    public readonly struct EnemyGainBuffRequested
    {
        public readonly string BuffId;
        public readonly int Value;

        public EnemyGainBuffRequested(string buffId, int value)
        {
            BuffId = buffId;
            Value = value;
        }
    }

    public readonly struct EnemyChargeIfPlayerSkippedAttackRequested
    {
    }
}
