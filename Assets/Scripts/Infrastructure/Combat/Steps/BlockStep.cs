using FlushAndFury.Domain.Combat;
using System;

namespace FlushAndFury.Infrastructure.Combat.Steps
{
    public sealed class MitigationStep : IDamageStep
    {
        public int Order => 50;

        public void Execute(DamageContext context)
        {
            context.CurrentDamage = Math.Max(0f, context.CurrentDamage - context.PipelineBlock);
        }
    }
}
