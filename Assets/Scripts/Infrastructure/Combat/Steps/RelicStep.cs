using FlushAndFury.Config.Relics;
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
        private readonly RelicRuleDatabase relicRuleDatabase;

        public int Order => 20;

        public RelicStep(IEventBus bus, RelicRuleDatabase database)
        {
            eventBus = bus;
            relicRuleDatabase = database;
        }

        public void Execute(DamageContext context)
        {
            if (relicRuleDatabase == null)
            {
                ExecuteFallback(context);
                return;
            }

            List<RelicRuleDefinition> activeRelics = relicRuleDatabase != null
                ? relicRuleDatabase.GetRules(context.ActiveRelicIds)
                : new List<RelicRuleDefinition>();

            for (int i = 0; i < activeRelics.Count; i++)
            {
                RelicRuleDefinition relic = activeRelics[i];
                if (!PassConditions(context, relic))
                {
                    continue;
                }

                if (relic.ActionType == RelicRuleActionType.ApplyPlayerStatus)
                {
                    string statusId = string.IsNullOrWhiteSpace(relic.StringValue) ? "Strength" : relic.StringValue;
                    eventBus?.Publish(new ApplyStatusToPlayerRequested(statusId, relic.IntValue, relic.DurationTurns));
                }
                else if (relic.ActionType == RelicRuleActionType.ModifyDamageMultiplier)
                {
                    context.Relic.DamageMultiplier *= Math.Max(0f, relic.FloatValue);
                }
                else if (relic.ActionType == RelicRuleActionType.DealRandomEnemyDamage)
                {
                    eventBus?.Publish(new DealRandomEnemyDamageRequested(relic.IntValue));
                }
                else if (relic.ActionType == RelicRuleActionType.DrawCard)
                {
                    eventBus?.Publish(new DrawCardRequested(relic.IntValue));
                }
            }

            float multiplier = Math.Max(0f, context.Relic.DamageMultiplier);
            context.CurrentDamage = (context.CurrentDamage + context.Relic.FlatDamageBonus) * multiplier;
        }

        private void ExecuteFallback(DamageContext context)
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
                    if (context.HandPattern == HandPattern.Pair && context.PlayedCardTags.Any(tag => string.Equals(tag, CombatModifierIds.TagWarrior, StringComparison.OrdinalIgnoreCase)))
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

        private bool PassConditions(DamageContext context, RelicRuleDefinition relic)
        {
            if (relic.RequireMagicDamage && context.DamageType != DamageType.Magic)
            {
                return false;
            }

            if (relic.RequireHpLostThisTurn && context.HpLostThisTurn <= 0)
            {
                return false;
            }

            if (relic.RequireDiscardThisTurn && context.DiscardCountThisTurn <= 0)
            {
                return false;
            }

            if (relic.UseHandPattern && context.HandPattern != relic.RequiredHandPattern)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(relic.RequiredTagId) && !context.PlayedCardTags.Any(tag => string.Equals(tag, relic.RequiredTagId, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            return true;
        }
    }
}
