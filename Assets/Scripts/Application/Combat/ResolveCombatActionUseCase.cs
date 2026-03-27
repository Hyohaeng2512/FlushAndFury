using FlushAndFury.Domain.Combat;

namespace FlushAndFury.Application.Combat
{
    public sealed class ResolveCombatActionUseCase
    {
        private readonly ICombatCalculator combatCalculator;

        public ResolveCombatActionUseCase(ICombatCalculator calculator)
        {
            combatCalculator = calculator;
        }

        public DamageContext Execute(CombatActionCommand command)
        {
            DamageContext context = new DamageContext(
                command.BaseDamage,
                command.PipelineBlock,
                command.DamageType
            );

            context.CardBuff.FlatDamageBonus = command.CardFlatDamageBonus;
            context.CardBuff.DamageMultiplier = command.CardDamageMultiplier;

            context.Relic.FlatDamageBonus = command.RelicFlatDamageBonus;
            context.Relic.DamageMultiplier = command.RelicDamageMultiplier;

            context.Boon.FlatDamageBonus = command.BoonFlatDamageBonus;
            context.Boon.DamageMultiplier = command.BoonDamageMultiplier;

            context.IsImmune = command.IsImmune;
            context.DefenseMultiplier = command.DefenseMultiplier;
            context.HandPattern = command.HandPattern;
            context.DiscardCountThisTurn = command.DiscardCountThisTurn;
            context.HpLostThisTurn = command.HpLostThisTurn;
            context.PlayerSkippedAttack = command.PlayerSkippedAttack;

            context.EnemyDefenseRule = command.EnemyDefenseRule;
            context.EnemyPhaseMask = command.EnemyPhaseMask;
            context.HitsTakenThisTurn = command.HitsTakenThisTurn;
            context.ReflectThreshold = command.ReflectThreshold;

            context.PlayedCardTags = command.PlayedCardTags ?? context.PlayedCardTags;
            context.PlayedCardEnchants = command.PlayedCardEnchants ?? context.PlayedCardEnchants;
            context.PlayedCardSeals = command.PlayedCardSeals ?? context.PlayedCardSeals;
            context.ActiveRelicIds = command.ActiveRelicIds ?? context.ActiveRelicIds;

            return combatCalculator.Resolve(context);
        }
    }
}
