using FlushAndFury.Application.Combat;
using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using FlushAndFury.Infrastructure.Random;
using FlushAndFury.Presentation.Battle;
using UnityEngine;

namespace FlushAndFury.Core.DI
{
    public class BattleLifetimeScope : MonoBehaviour
    {
        private ICombatCalculator combatCalculator;
        private IEventBus eventBus;
        private IRngService rngService;

        public ResolveCombatActionUseCase ResolveCombatActionUseCase { get; private set; }
        public CombatTurnFlowService CombatTurnFlowService { get; private set; }

        public void Initialize(ICombatCalculator calculator, IEventBus bus, IRngService rng)
        {
            combatCalculator = calculator;
            eventBus = bus;
            rngService = rng;

            ResolveCombatActionUseCase = new ResolveCombatActionUseCase(combatCalculator);
            EnemyActionExecutor enemyActionExecutor = new EnemyActionExecutor(eventBus);
            EnemyTurnService enemyTurnService = new EnemyTurnService(rngService, eventBus, enemyActionExecutor);
            CombatTurnFlowService = new CombatTurnFlowService(ResolveCombatActionUseCase, enemyTurnService, eventBus);

            BattlePresenter presenter = FindAnyObjectByType<BattlePresenter>();
            presenter?.SetUseCase(ResolveCombatActionUseCase);
            presenter?.SetTurnFlowService(CombatTurnFlowService);

            CombatDebugListener debugListener = FindAnyObjectByType<CombatDebugListener>();
            debugListener?.Bind(eventBus);
        }
    }
}
