using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using System;

namespace FlushAndFury.Infrastructure.Combat.Steps
{
    public sealed class DefenseStep : IDamageStep
    {
        private readonly IEventBus eventBus;

        public int Order => 40;

        public DefenseStep(IEventBus bus)
        {
            eventBus = bus;
        }

        public void Execute(DamageContext context)
        {
            ApplyEnemyRule(context);

            if (context.IsImmune)
            {
                context.CurrentDamage = 0f;
                return;
            }

            float multiplier = Math.Max(0f, context.DefenseMultiplier);
            context.CurrentDamage *= multiplier;
        }

        private void ApplyEnemyRule(DamageContext context)
        {
            switch (context.EnemyDefenseRule)
            {
                case EnemyDefenseRule.BasicBrute:
                case EnemyDefenseRule.None:
                    break;
                case EnemyDefenseRule.ShieldGuardian:
                    if (context.HitsTakenThisTurn + 1 < 2)
                    {
                        context.IsImmune = true;
                    }
                    break;
                case EnemyDefenseRule.MirrorBeast:
                    if (context.CurrentDamage >= context.ReflectThreshold)
                    {
                        int reflectDamage = Math.Max(0, (int)Math.Floor(context.CurrentDamage * 0.5f));
                        eventBus?.Publish(new ReflectDamageToPlayerRequested(reflectDamage));
                    }
                    break;
                case EnemyDefenseRule.PhaseShifter:
                    if ((context.EnemyPhaseMask == EnemyPhaseMask.PhysicalOnly && context.DamageType != DamageType.Physical) ||
                        (context.EnemyPhaseMask == EnemyPhaseMask.MagicOnly && context.DamageType != DamageType.Magic))
                    {
                        context.IsImmune = true;
                    }
                    break;
                case EnemyDefenseRule.CardThief:
                    if (context.DiscardCountThisTurn > 0)
                    {
                        eventBus?.Publish(new EnemyGainBuffRequested("AttackUp", 1));
                    }
                    break;
                case EnemyDefenseRule.BurnDemon:
                    if (context.PlayerSkippedAttack)
                    {
                        eventBus?.Publish(new EnemyChargeIfPlayerSkippedAttackRequested());
                    }
                    break;
            }
        }
    }
}
