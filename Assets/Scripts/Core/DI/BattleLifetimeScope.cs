using FlushAndFury.Application.Combat;
using FlushAndFury.Application.Run;
using FlushAndFury.Config.Enemies;
using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using FlushAndFury.Infrastructure.Random;
using FlushAndFury.Presentation.Battle;
using UnityEngine;

namespace FlushAndFury.Core.DI
{
    public class BattleLifetimeScope : MonoBehaviour
    {
        [SerializeField] private int enemyMaxHp = 60;

        private ICombatCalculator combatCalculator;
        private IEventBus eventBus;
        private IRngService rngService;

        public ResolveCombatActionUseCase ResolveCombatActionUseCase { get; private set; }
        public CombatTurnFlowService CombatTurnFlowService { get; private set; }

        public void Initialize(ICombatCalculator calculator, IEventBus bus, IRngService rng, EnemyIntentProfile enemyIntentProfile, RunProgressService runProgressService)
        {
            combatCalculator = calculator;
            eventBus = bus;
            rngService = rng;

            ResolveCombatActionUseCase = new ResolveCombatActionUseCase(combatCalculator);
            CombatStatusService combatStatusService = new CombatStatusService(eventBus);
            CombatHealthService combatHealthService = new CombatHealthService(eventBus, runProgressService);
            EnemyActionExecutor enemyActionExecutor = new EnemyActionExecutor(eventBus, combatStatusService);
            EnemyTurnService enemyTurnService = new EnemyTurnService(rngService, eventBus, enemyActionExecutor, enemyIntentProfile);
            CombatTurnFlowService = new CombatTurnFlowService(ResolveCombatActionUseCase, enemyTurnService, combatStatusService, combatHealthService, eventBus, enemyMaxHp);

            BattlePresenter presenter = FindAnyObjectByType<BattlePresenter>();
            presenter?.SetUseCase(ResolveCombatActionUseCase);
            presenter?.SetTurnFlowService(CombatTurnFlowService);

            CombatDebugListener debugListener = FindAnyObjectByType<CombatDebugListener>();
            debugListener?.Bind(eventBus);
        }
    }
}
