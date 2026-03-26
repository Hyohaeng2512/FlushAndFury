using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlushAndFury.Infrastructure.Combat.Steps
{
    public sealed class RelicStep : IDamageStep
    {
        private readonly IEventBus eventBus;

        public int Order => 20;

        public RelicStep(IEventBus bus)
        {
            eventBus = bus;
        }

        public void Execute(DamageContext context)
        {
            List<string> activeRelics = context.ActiveRelicIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            for (int i = 0; i < activeRelics.Count; i++)
            {
                string relicId = activeRelics[i];

                if (relicId == CombatModifierIds.RelicWarriorEmblem)
                {
                    if (context.HandPattern == HandPattern.Pair && HasTag(context, CombatModifierIds.TagWarrior))
                    {
                        eventBus?.Publish(new ApplyStatusToPlayerRequested("Strength", 2, 2));
                    }
                }
                else if (relicId == CombatModifierIds.RelicArcaneCore)
                {
                    if (context.DamageType == DamageType.Magic)
                    {
                        context.Relic.DamageMultiplier *= 1.3f;
                    }
                }
                else if (relicId == CombatModifierIds.RelicBloodPact)
                {
                    if (context.HpLostThisTurn > 0)
                    {
                        eventBus?.Publish(new ApplyStatusToPlayerRequested("Strength", 1, -1));
                    }
                }
                else if (relicId == CombatModifierIds.RelicGamblersCoin)
                {
                    if (context.DiscardCountThisTurn > 0)
                    {
                        eventBus?.Publish(new DealRandomEnemyDamageRequested(5));
                    }
                }
                else if (relicId == CombatModifierIds.RelicPerfectFlow)
                {
                    if (context.HandPattern == HandPattern.Straight)
                    {
                        eventBus?.Publish(new DrawCardRequested(1));
                    }
                }
            }

            float multiplier = Math.Max(0f, context.Relic.DamageMultiplier);
            context.CurrentDamage = (context.CurrentDamage + context.Relic.FlatDamageBonus) * multiplier;
        }

        private bool HasTag(DamageContext context, string tagId)
        {
            return context.PlayedCardTags.Any(tag => string.Equals(tag, tagId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
