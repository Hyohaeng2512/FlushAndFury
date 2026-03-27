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

    public readonly struct EnemyIntentSelected
    {
        public readonly string IntentType;
        public readonly int Value;
        public readonly string Description;

        public EnemyIntentSelected(string intentType, int value, string description)
        {
            IntentType = intentType;
            Value = value;
            Description = description;
        }
    }

    public readonly struct EnemyIntentTelegraphed
    {
        public readonly string IntentType;
        public readonly int Value;
        public readonly string Description;

        public EnemyIntentTelegraphed(string intentType, int value, string description)
        {
            IntentType = intentType;
            Value = value;
            Description = description;
        }
    }

    public readonly struct EnemyIntentConsumed
    {
        public readonly string IntentType;
        public readonly int Value;
        public readonly int TurnIndex;

        public EnemyIntentConsumed(string intentType, int value, int turnIndex)
        {
            IntentType = intentType;
            Value = value;
            TurnIndex = turnIndex;
        }
    }

    public readonly struct TurnSnapshotRecorded
    {
        public readonly string Source;
        public readonly int TurnIndex;
        public readonly string Owner;
        public readonly string Phase;

        public TurnSnapshotRecorded(string source, int turnIndex, string owner, string phase)
        {
            Source = source;
            TurnIndex = turnIndex;
            Owner = owner;
            Phase = phase;
        }
    }

    public readonly struct CombatStatusApplied
    {
        public readonly string Side;
        public readonly string StatusId;
        public readonly int Stacks;
        public readonly int RemainingTurns;
        public readonly bool IsPermanent;

        public CombatStatusApplied(string side, string statusId, int stacks, int remainingTurns, bool isPermanent)
        {
            Side = side;
            StatusId = statusId;
            Stacks = stacks;
            RemainingTurns = remainingTurns;
            IsPermanent = isPermanent;
        }
    }

    public readonly struct CombatStatusExpired
    {
        public readonly string Side;
        public readonly string StatusId;

        public CombatStatusExpired(string side, string statusId)
        {
            Side = side;
            StatusId = statusId;
        }
    }

    public readonly struct CombatStatusTickDamage
    {
        public readonly string Side;
        public readonly string StatusId;
        public readonly int Damage;
        public readonly int TurnIndex;

        public CombatStatusTickDamage(string side, string statusId, int damage, int turnIndex)
        {
            Side = side;
            StatusId = statusId;
            Damage = damage;
            TurnIndex = turnIndex;
        }
    }

    public readonly struct EnemyAttackResolved
    {
        public readonly int Damage;

        public EnemyAttackResolved(int damage)
        {
            Damage = damage;
        }
    }

    public readonly struct EnemyBlockGained
    {
        public readonly int Amount;

        public EnemyBlockGained(int amount)
        {
            Amount = amount;
        }
    }

    public readonly struct EnemyDebuffApplied
    {
        public readonly string DebuffId;
        public readonly int Value;

        public EnemyDebuffApplied(string debuffId, int value)
        {
            DebuffId = debuffId;
            Value = value;
        }
    }

    public readonly struct RunStateChanged
    {
        public readonly int MaxHp;
        public readonly int CurrentHp;
        public readonly int Gold;
        public readonly string Reason;

        public RunStateChanged(int maxHp, int currentHp, int gold, string reason)
        {
            MaxHp = maxHp;
            CurrentHp = currentHp;
            Gold = gold;
            Reason = reason;
        }
    }

    public readonly struct CombatHealthChanged
    {
        public readonly string Side;
        public readonly int CurrentHp;
        public readonly int MaxHp;
        public readonly int DamageTaken;
        public readonly string Reason;

        public CombatHealthChanged(string side, int currentHp, int maxHp, int damageTaken, string reason)
        {
            Side = side;
            CurrentHp = currentHp;
            MaxHp = maxHp;
            DamageTaken = damageTaken;
            Reason = reason;
        }
    }

    public readonly struct CombatBlockChanged
    {
        public readonly string Side;
        public readonly int CurrentBlock;
        public readonly string Reason;

        public CombatBlockChanged(string side, int currentBlock, string reason)
        {
            Side = side;
            CurrentBlock = currentBlock;
            Reason = reason;
        }
    }

    public readonly struct CombatEntityDefeated
    {
        public readonly string Side;
        public readonly string Reason;

        public CombatEntityDefeated(string side, string reason)
        {
            Side = side;
            Reason = reason;
        }
    }

    public readonly struct BattleEnded
    {
        public readonly string Winner;
        public readonly int TurnIndex;
        public readonly string Reason;

        public BattleEnded(string winner, int turnIndex, string reason)
        {
            Winner = winner;
            TurnIndex = turnIndex;
            Reason = reason;
        }
    }

    public readonly struct StageCleared
    {
        public readonly int StageIndex;
        public readonly int RewardGold;
        public readonly int TotalGold;

        public StageCleared(int stageIndex, int rewardGold, int totalGold)
        {
            StageIndex = stageIndex;
            RewardGold = rewardGold;
            TotalGold = totalGold;
        }
    }

    public readonly struct MapStarted
    {
        public readonly int NodeCount;
        public readonly string StartNodeId;

        public MapStarted(int nodeCount, string startNodeId)
        {
            NodeCount = nodeCount;
            StartNodeId = startNodeId;
        }
    }

    public readonly struct MapNodeUnlocked
    {
        public readonly string NodeId;
        public readonly string NodeType;

        public MapNodeUnlocked(string nodeId, string nodeType)
        {
            NodeId = nodeId;
            NodeType = nodeType;
        }
    }

    public readonly struct MapNodeEntered
    {
        public readonly string NodeId;
        public readonly string NodeType;

        public MapNodeEntered(string nodeId, string nodeType)
        {
            NodeId = nodeId;
            NodeType = nodeType;
        }
    }

    public readonly struct MapAwaitingCombat
    {
        public readonly string NodeId;
        public readonly string NodeType;

        public MapAwaitingCombat(string nodeId, string nodeType)
        {
            NodeId = nodeId;
            NodeType = nodeType;
        }
    }

    public readonly struct MapNodeCleared
    {
        public readonly string NodeId;
        public readonly string NodeType;
        public readonly string Reason;

        public MapNodeCleared(string nodeId, string nodeType, string reason)
        {
            NodeId = nodeId;
            NodeType = nodeType;
            Reason = reason;
        }
    }

    public readonly struct MapRunFailed
    {
        public readonly string NodeId;
        public readonly string Reason;

        public MapRunFailed(string nodeId, string reason)
        {
            NodeId = nodeId;
            Reason = reason;
        }
    }

    public readonly struct MapCompleted
    {
        public readonly string BossNodeId;

        public MapCompleted(string bossNodeId)
        {
            BossNodeId = bossNodeId;
        }
    }
}
