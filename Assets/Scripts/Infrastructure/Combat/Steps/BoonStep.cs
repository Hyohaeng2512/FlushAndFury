using FlushAndFury.Domain.Combat;
using System;

namespace FlushAndFury.Infrastructure.Combat.Steps
{
    public sealed class BoonStep : IDamageStep
    {
        public int Order => 30;

        public void Execute(DamageContext context)
        {
            float multiplier = Math.Max(0f, context.Boon.DamageMultiplier);
            context.CurrentDamage = (context.CurrentDamage + context.Boon.FlatDamageBonus) * multiplier;
        }
    }
}
