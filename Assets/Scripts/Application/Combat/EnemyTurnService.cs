using FlushAndFury.Config.Enemies;
using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using FlushAndFury.Infrastructure.Random;
using System.Collections.Generic;
using UnityEngine;

namespace FlushAndFury.Application.Combat
{
    public sealed class EnemyTurnService
    {
        private readonly IRngService rngService;
        private readonly IEventBus eventBus;
        private readonly EnemyActionExecutor enemyActionExecutor;
        private readonly EnemyIntentProfile intentProfile;
        private EnemyIntent telegraphedIntent;
        private int telegraphedTurnIndex = -1;
        private int lastResolvedTurnIndex = -1;

        public EnemyTurnService(IRngService rng, IEventBus bus, EnemyActionExecutor actionExecutor, EnemyIntentProfile profile)
        {
            rngService = rng;
            eventBus = bus;
            enemyActionExecutor = actionExecutor;
            intentProfile = profile;
        }

        public void InitializeForBattle(int turnIndex)
        {
            telegraphedIntent = SelectIntent(turnIndex);
            telegraphedTurnIndex = turnIndex;
            lastResolvedTurnIndex = -1;
            PublishTelegraph(telegraphedIntent);
        }

        public EnemyTurnResult ResolveTurn(int turnIndex)
        {
            if (turnIndex <= 0)
            {
                Debug.LogWarning($"[EnemyTurnService] Invalid turn index: {turnIndex}");
                return null;
            }

            if (lastResolvedTurnIndex == turnIndex)
            {
                Debug.LogWarning($"[EnemyTurnService] Enemy turn already resolved for turn {turnIndex}");
                return null;
            }

            if (telegraphedIntent == null)
            {
                InitializeForBattle(turnIndex);
            }

            if (telegraphedTurnIndex != turnIndex)
            {
                Debug.LogWarning($"[EnemyTurnService] Telegraph mismatch. expected={telegraphedTurnIndex} incoming={turnIndex}. Regenerating.");
                telegraphedIntent = SelectIntent(turnIndex);
                telegraphedTurnIndex = turnIndex;
                PublishTelegraph(telegraphedIntent);
            }

            EnemyIntent intent = telegraphedIntent;
            eventBus?.Publish(new EnemyIntentSelected(intent.IntentType.ToString(), intent.Value, intent.Description));
            eventBus?.Publish(new EnemyIntentConsumed(intent.IntentType.ToString(), intent.Value, turnIndex));

            EnemyTurnResult result = enemyActionExecutor.Execute(intent);
            lastResolvedTurnIndex = turnIndex;

            telegraphedIntent = SelectIntent(turnIndex + 1);
            telegraphedTurnIndex = turnIndex + 1;
            PublishTelegraph(telegraphedIntent);

            return result;
        }

        private void PublishTelegraph(EnemyIntent intent)
        {
            eventBus?.Publish(new EnemyIntentTelegraphed(intent.IntentType.ToString(), intent.Value, intent.Description));
        }

        private EnemyIntent SelectIntent(int turnIndex)
        {
            if (intentProfile != null && intentProfile.Options != null && intentProfile.Options.Count > 0)
            {
                EnemyIntent fromProfile = SelectIntentFromProfile();
                if (fromProfile != null)
                {
                    return fromProfile;
                }
            }

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

        private EnemyIntent SelectIntentFromProfile()
        {
            int totalWeight = 0;
            List<EnemyIntentOption> options = intentProfile.Options;
            for (int i = 0; i < options.Count; i++)
            {
                EnemyIntentOption option = options[i];
                if (option == null || option.weight <= 0)
                {
                    continue;
                }

                totalWeight += option.weight;
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int roll = rngService.NextInt(0, totalWeight);
            int cursor = 0;
            for (int i = 0; i < options.Count; i++)
            {
                EnemyIntentOption option = options[i];
                if (option == null || option.weight <= 0)
                {
                    continue;
                }

                cursor += option.weight;
                if (roll < cursor)
                {
                    int min = Mathf.Min(option.minValue, option.maxValue);
                    int max = Mathf.Max(option.minValue, option.maxValue);
                    int value = rngService.NextInt(min, max + 1);

                    return new EnemyIntent
                    {
                        IntentType = option.intentType,
                        Value = value,
                        Description = option.description,
                    };
                }
            }

            return null;
        }
    }
}
