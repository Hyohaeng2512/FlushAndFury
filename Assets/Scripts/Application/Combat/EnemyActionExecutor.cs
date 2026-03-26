using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;

namespace FlushAndFury.Application.Combat
{
    public sealed class EnemyActionExecutor
    {
        private readonly IEventBus eventBus;

        public EnemyActionExecutor(IEventBus bus)
        {
            eventBus = bus;
        }

        public EnemyTurnResult Execute(EnemyIntent intent)
        {
            EnemyTurnResult result = new EnemyTurnResult
            {
                Intent = intent,
            };

            if (intent.IntentType == EnemyIntentType.Attack)
            {
                result.DamageToPlayer = intent.Value;
                eventBus?.Publish(new EnemyAttackResolved(intent.Value));
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
