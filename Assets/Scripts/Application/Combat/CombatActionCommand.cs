using FlushAndFury.Domain.Combat;
using System.Collections.Generic;

namespace FlushAndFury.Application.Combat
{
    public sealed class CombatActionCommand
    {
        public int BaseDamage { get; set; }
        public int TargetBlock { get; set; }
        public DamageType DamageType { get; set; } = DamageType.Physical;

        public int CardFlatDamageBonus { get; set; }
        public float CardDamageMultiplier { get; set; } = 1f;

        public int RelicFlatDamageBonus { get; set; }
        public float RelicDamageMultiplier { get; set; } = 1f;

        public int BoonFlatDamageBonus { get; set; }
        public float BoonDamageMultiplier { get; set; } = 1f;

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

        public List<string> PlayedCardTags { get; set; } = new List<string>();
        public List<string> PlayedCardEnchants { get; set; } = new List<string>();
        public List<string> PlayedCardSeals { get; set; } = new List<string>();
        public List<string> ActiveRelicIds { get; set; } = new List<string>();
    }
}
