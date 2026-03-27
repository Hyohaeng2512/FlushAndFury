using System.Collections.Generic;

namespace FlushAndFury.Domain.Combat
{
    public enum DamageType
    {
        Physical = 0,
        Magic = 1,
    }

    public sealed class DamageContext
    {
        public int BaseDamage { get; set; }
        public float CurrentDamage { get; set; }
        public int PipelineBlock { get; set; }
        public int FinalDamage { get; set; }
        public DamageType DamageType { get; set; }
        public bool IsImmune { get; set; }
        public float DefenseMultiplier { get; set; } = 1f;
        public HandPattern HandPattern { get; set; } = HandPattern.HighCard;
        public int DiscardCountThisTurn { get; set; }
        public int HpLostThisTurn { get; set; }
        public bool PlayerSkippedAttack { get; set; }

        public EnemyDefenseRule EnemyDefenseRule { get; set; } = EnemyDefenseRule.None;
        public EnemyPhaseMask EnemyPhaseMask { get; set; } = EnemyPhaseMask.AcceptAll;
        public int HitsTakenThisTurn { get; set; }
        public int ReflectThreshold { get; set; } = 15;

        public List<string> PlayedCardTags { get; set; }
        public List<string> PlayedCardEnchants { get; set; }
        public List<string> PlayedCardSeals { get; set; }
        public List<string> ActiveRelicIds { get; set; }

        public CardBuffContext CardBuff { get; set; }
        public RelicContext Relic { get; set; }
        public BoonContext Boon { get; set; }

        public DamageContext(int baseDamage, int pipelineBlock, DamageType damageType)
        {
            BaseDamage = baseDamage;
            CurrentDamage = baseDamage;
            PipelineBlock = pipelineBlock;
            DamageType = damageType;
            CardBuff = new CardBuffContext();
            Relic = new RelicContext();
            Boon = new BoonContext();
            PlayedCardTags = new List<string>();
            PlayedCardEnchants = new List<string>();
            PlayedCardSeals = new List<string>();
            ActiveRelicIds = new List<string>();
        }
    }
}
