using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using FlushAndFury.Infrastructure.Random;

namespace FlushAndFury.Application.Combat
{
    public sealed class EnemyTurnService
    {
        private readonly IRngService rngService;
        private readonly IEventBus eventBus;
        private EnemyIntent telegraphedIntent;

        public EnemyTurnService(IRngService rng, IEventBus bus)
        {
            rngService = rng;
            eventBus = bus;
        }

        public void InitializeForBattle(int turnIndex)
        {
            telegraphedIntent = SelectIntent(turnIndex);
            PublishTelegraph(telegraphedIntent);
        }

        public EnemyTurnResult ResolveTurn(int turnIndex)
        {
            if (telegraphedIntent == null)
            {
                InitializeForBattle(turnIndex);
            }

            EnemyIntent intent = telegraphedIntent;
            eventBus?.Publish(new EnemyIntentSelected(intent.IntentType.ToString(), intent.Value, intent.Description));

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

            telegraphedIntent = SelectIntent(turnIndex + 1);
            PublishTelegraph(telegraphedIntent);

            return result;
        }

        private void PublishTelegraph(EnemyIntent intent)
        {
            eventBus?.Publish(new EnemyIntentTelegraphed(intent.IntentType.ToString(), intent.Value, intent.Description));
        }

        private EnemyIntent SelectIntent(int turnIndex)
        {
            int roll = rngService.NextInt(0, 3);

            if ((turnIndex + roll) % 3 == 1)
            {
                return new EnemyIntent
                {
                    IntentType = EnemyIntentType.Attack,
                    Value = 7 + rngService.NextInt(0, 4),
                    Description = "Direct attack",
                };
            }

            if ((turnIndex + roll) % 3 == 2)
            {
                return new EnemyIntent
                {
                    IntentType = EnemyIntentType.Defend,
                    Value = 6,
                    Description = "Gain block",
                };
            }

            return new EnemyIntent
            {
                IntentType = EnemyIntentType.Debuff,
                Value = 1,
                Description = "Apply weak",
            };
        }
    }
}
