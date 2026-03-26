using FlushAndFury.Config.Cards;
using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace FlushAndFury.Infrastructure.Combat.Steps
{
    public sealed class CardBuffStep : IDamageStep
    {
        private readonly IEventBus eventBus;
        private readonly CardRuleDatabase cardRuleDatabase;

        public int Order => 10;

        public CardBuffStep(IEventBus bus, CardRuleDatabase database)
        {
            eventBus = bus;
            cardRuleDatabase = database;
        }

        public void Execute(DamageContext context)
        {
            ApplyEnchants(context);
            ApplyTags(context);
            ApplySeals(context);

            float multiplier = Math.Max(0f, context.CardBuff.DamageMultiplier);
            context.CurrentDamage = (context.CurrentDamage + context.CardBuff.FlatDamageBonus) * multiplier;
        }

        private void ApplyEnchants(DamageContext context)
        {
            if (cardRuleDatabase == null)
            {
                ApplyEnchantsFallback(context);
                return;
            }

            List<CardRuleDefinition> enchants = cardRuleDatabase != null
                ? cardRuleDatabase.GetRules(context.PlayedCardEnchants, CardRuleCategory.Enchant)
                : new List<CardRuleDefinition>();

            for (int i = 0; i < enchants.Count; i++)
            {
                CardRuleDefinition rule = enchants[i];
                if (rule.EffectType == CardRuleEffectType.FlatDamageBonus)
                {
                    context.CardBuff.FlatDamageBonus += rule.IntValue;
                }
                else if (rule.EffectType == CardRuleEffectType.PublishEnergyGain)
                {
                    eventBus?.Publish(new EnergyGainRequested(rule.IntValue));
                }
                else if (rule.EffectType == CardRuleEffectType.PublishApplyStatusToTarget)
                {
                    string statusId = string.IsNullOrWhiteSpace(rule.StringValue) ? "Burn" : rule.StringValue;
                    eventBus?.Publish(new ApplyStatusToTargetRequested(statusId, rule.IntValue));
                }
                else if (rule.EffectType == CardRuleEffectType.ConvertToMagic)
                {
                    context.DamageType = DamageType.Magic;
                }
                else if (rule.EffectType == CardRuleEffectType.DamageMultiplier)
                {
                    context.CardBuff.DamageMultiplier *= Math.Max(0f, rule.FloatValue);
                }
            }
        }

        private void ApplyTags(DamageContext context)
        {
            if (cardRuleDatabase == null)
            {
                ApplyTagsFallback(context);
                return;
            }

            List<CardRuleDefinition> tags = cardRuleDatabase != null
                ? cardRuleDatabase.GetRules(context.PlayedCardTags, CardRuleCategory.Tag)
                : new List<CardRuleDefinition>();

            for (int i = 0; i < tags.Count; i++)
            {
                CardRuleDefinition rule = tags[i];
                if (rule.EffectType == CardRuleEffectType.FlatDamageBonus)
                {
                    context.CardBuff.FlatDamageBonus += rule.IntValue;
                }
                else if (rule.EffectType == CardRuleEffectType.DamageMultiplier)
                {
                    context.CardBuff.DamageMultiplier *= Math.Max(0f, rule.FloatValue);
                }
                else if (rule.EffectType == CardRuleEffectType.PublishLuckyProc)
                {
                    eventBus?.Publish(new LuckyProcCheckRequested());
                }

            }
        }

        private void ApplySeals(DamageContext context)
        {
            if (cardRuleDatabase == null)
            {
                ApplySealsFallback(context);
                return;
            }

            List<CardRuleDefinition> seals = cardRuleDatabase != null
                ? cardRuleDatabase.GetRules(context.PlayedCardSeals, CardRuleCategory.Seal)
                : new List<CardRuleDefinition>();

            for (int i = 0; i < seals.Count; i++)
            {
                CardRuleDefinition rule = seals[i];
                if (rule.EffectType == CardRuleEffectType.PublishCardReturnToDeck)
                {
                    eventBus?.Publish(new CardReturnToDeckRequested());
                }
                else if (rule.EffectType == CardRuleEffectType.PublishGoldOnKillFlag)
                {
                    eventBus?.Publish(new GoldOnKillFlagRequested());
                }
                else if (rule.EffectType == CardRuleEffectType.PublishCardPlaceTopDeck)
                {
                    eventBus?.Publish(new CardPlaceTopDeckRequested());
                }
            }
        }

        private void ApplyEnchantsFallback(DamageContext context)
        {
            for (int i = 0; i < context.PlayedCardEnchants.Count; i++)
            {
                string enchantId = context.PlayedCardEnchants[i];
                if (string.Equals(enchantId, CombatModifierIds.EnchantFlat3, StringComparison.OrdinalIgnoreCase))
                {
                    context.CardBuff.FlatDamageBonus += 3;
                }
                else if (string.Equals(enchantId, CombatModifierIds.EnchantEnergy1, StringComparison.OrdinalIgnoreCase))
                {
                    eventBus?.Publish(new EnergyGainRequested(1));
                }
                else if (string.Equals(enchantId, CombatModifierIds.EnchantBurn2, StringComparison.OrdinalIgnoreCase))
                {
                    eventBus?.Publish(new ApplyStatusToTargetRequested("Burn", 2));
                }
                else if (string.Equals(enchantId, CombatModifierIds.EnchantConvertToMagic, StringComparison.OrdinalIgnoreCase))
                {
                    context.DamageType = DamageType.Magic;
                }
            }
        }

        private void ApplyTagsFallback(DamageContext context)
        {
            HashSet<string> tags = new HashSet<string>(context.PlayedCardTags, StringComparer.OrdinalIgnoreCase);
            if (tags.Contains(CombatModifierIds.TagWarrior))
            {
                context.CardBuff.FlatDamageBonus += 1;
            }

            if (tags.Contains(CombatModifierIds.TagArcane) && context.DamageType == DamageType.Magic)
            {
                context.CardBuff.DamageMultiplier *= 1.1f;
            }

            if (tags.Contains(CombatModifierIds.TagLucky))
            {
                eventBus?.Publish(new LuckyProcCheckRequested());
            }
        }

        private void ApplySealsFallback(DamageContext context)
        {
            HashSet<string> seals = new HashSet<string>(context.PlayedCardSeals, StringComparer.OrdinalIgnoreCase);
            if (seals.Contains(CombatModifierIds.SealReturn))
            {
                eventBus?.Publish(new CardReturnToDeckRequested());
            }

            if (seals.Contains(CombatModifierIds.SealGoldOnKill))
            {
                eventBus?.Publish(new GoldOnKillFlagRequested());
            }

            if (seals.Contains(CombatModifierIds.SealTopDeck))
            {
                eventBus?.Publish(new CardPlaceTopDeckRequested());
            }
        }
    }
}
