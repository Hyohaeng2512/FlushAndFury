using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using System;

namespace FlushAndFury.Application.Combat
{
    public sealed class EnemyActionExecutor
    {
        private readonly IEventBus eventBus;
        private readonly CombatStatusService statusService;

        public EnemyActionExecutor(IEventBus bus, CombatStatusService combatStatusService)
        {
            eventBus = bus;
            statusService = combatStatusService;
        }

        public EnemyTurnResult Execute(EnemyIntent intent)
        {
            EnemyTurnResult result = new EnemyTurnResult
            {
                Intent = intent,
            };

            if (intent.IntentType == EnemyIntentType.Attack)
            {
                int outgoingFlat = statusService.GetOutgoingFlatBonus(CombatSide.Enemy, DamageType.Physical);
                float outgoingMult = statusService.GetOutgoingMultiplier(CombatSide.Enemy);
                float incomingMult = statusService.GetIncomingMultiplier(CombatSide.Player);

                float attackDamage = (intent.Value + outgoingFlat) * outgoingMult * incomingMult;
                result.DamageToPlayer = Math.Max(0, (int)Math.Floor(attackDamage));
                eventBus?.Publish(new EnemyAttackResolved(result.DamageToPlayer));
            }
            else if (intent.IntentType == EnemyIntentType.Defend)
            {
                result.BlockGained = intent.Value;
                eventBus?.Publish(new EnemyBlockGained(intent.Value));
            }
            else if (intent.IntentType == EnemyIntentType.Debuff)
            {
                result.DebuffApplied = true;
                eventBus?.Publish(new EnemyDebuffApplied("Weak", intent.Value));
            }

            return result;
        }
    }
}
