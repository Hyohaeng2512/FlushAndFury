using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlushAndFury.Infrastructure.Combat.Steps
{
    public sealed class CardBuffStep : IDamageStep
    {
        private readonly IEventBus eventBus;

        public int Order => 10;

        public CardBuffStep(IEventBus bus)
        {
            eventBus = bus;
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
            List<string> enchants = context.PlayedCardEnchants
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .OrderBy(GetEnchantPriority)
                .ToList();

            for (int i = 0; i < enchants.Count; i++)
            {
                string enchantId = enchants[i];
                if (enchantId == CombatModifierIds.EnchantFlat3)
                {
                    context.CardBuff.FlatDamageBonus += 3;
                }
                else if (enchantId == CombatModifierIds.EnchantEnergy1)
                {
                    eventBus?.Publish(new EnergyGainRequested(1));
                }
                else if (enchantId == CombatModifierIds.EnchantBurn2)
                {
                    eventBus?.Publish(new ApplyStatusToTargetRequested("Burn", 2));
                }
                else if (enchantId == CombatModifierIds.EnchantConvertToMagic)
                {
                    context.DamageType = DamageType.Magic;
                }
            }
        }

        private void ApplyTags(DamageContext context)
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

        private void ApplySeals(DamageContext context)
        {
            List<string> seals = context.PlayedCardSeals
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .OrderBy(GetSealPriority)
                .ToList();

            for (int i = 0; i < seals.Count; i++)
            {
                string sealId = seals[i];
                if (sealId == CombatModifierIds.SealReturn)
                {
                    eventBus?.Publish(new CardReturnToDeckRequested());
                }
                else if (sealId == CombatModifierIds.SealGoldOnKill)
                {
                    eventBus?.Publish(new GoldOnKillFlagRequested());
                }
                else if (sealId == CombatModifierIds.SealTopDeck)
                {
                    eventBus?.Publish(new CardPlaceTopDeckRequested());
                }
            }
        }

        private int GetEnchantPriority(string enchantId)
        {
            if (enchantId == CombatModifierIds.EnchantFlat3) return 100;
            if (enchantId == CombatModifierIds.EnchantEnergy1) return 110;
            if (enchantId == CombatModifierIds.EnchantBurn2) return 120;
            if (enchantId == CombatModifierIds.EnchantConvertToMagic) return 130;

            return 999;
        }

        private int GetSealPriority(string sealId)
        {
            if (sealId == CombatModifierIds.SealReturn) return 300;
            if (sealId == CombatModifierIds.SealGoldOnKill) return 310;
            if (sealId == CombatModifierIds.SealTopDeck) return 320;

            return 999;
        }
    }
}
