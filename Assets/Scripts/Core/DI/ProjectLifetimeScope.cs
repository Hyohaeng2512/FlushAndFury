using FlushAndFury.Application.Combat;
using FlushAndFury.Config.Cards;
using FlushAndFury.Config.Enemies;
using FlushAndFury.Config.Relics;
using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Combat;
using FlushAndFury.Infrastructure.Combat.Steps;
using FlushAndFury.Infrastructure.Events;
using FlushAndFury.Infrastructure.Random;
using System.Collections.Generic;
using UnityEngine;

namespace FlushAndFury.Core.DI
{
    public class ProjectLifetimeScope : MonoBehaviour
    {
        [SerializeField] private BattleLifetimeScope battleLifetimeScopePrefab;
        [SerializeField] private CardRuleDatabase cardRuleDatabase;
        [SerializeField] private RelicRuleDatabase relicRuleDatabase;
        [SerializeField] private EnemyIntentProfile enemyIntentProfile;

        private IRngService rngService;
        private IEventBus eventBus;

        public void Bootstrap()
        {
            rngService = new SeededRngService(123456);
            eventBus = new EventBus();

            if (battleLifetimeScopePrefab == null)
            {
                return;
            }

            BattleLifetimeScope battleScope = Instantiate(battleLifetimeScopePrefab);
            battleScope.Initialize(CreateCombatCalculator(), eventBus, rngService, enemyIntentProfile);
        }

        private ICombatCalculator CreateCombatCalculator()
        {
            List<IDamageStep> steps = new List<IDamageStep>
            {
                new CardBuffStep(eventBus, cardRuleDatabase),
                new RelicStep(eventBus, relicRuleDatabase),
                new BoonStep(),
                new DefenseStep(eventBus),
                new BlockStep(),
                new FinalizeStep(),
            };

            return new CombatCalculator(steps);
        }
    }
}
