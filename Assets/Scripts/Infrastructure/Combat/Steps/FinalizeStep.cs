using FlushAndFury.Domain.Combat;
using System;

namespace FlushAndFury.Infrastructure.Combat.Steps
{
    public sealed class FinalizeStep : IDamageStep
    {
        public int Order => 60;

        public void Execute(DamageContext context)
        {
            context.FinalDamage = Math.Max(0, (int)Math.Floor(context.CurrentDamage));
            context.CurrentDamage = context.FinalDamage;
        }
    }
}
