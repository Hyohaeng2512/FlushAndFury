using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using UnityEngine;

namespace FlushAndFury.Application.Combat
{
    public sealed class CombatTurnFlowService
    {
        private readonly ResolveCombatActionUseCase resolveCombatActionUseCase;
        private readonly EnemyTurnService enemyTurnService;
        private readonly CombatStatusService statusService;
        private readonly CombatHealthService healthService;
        private readonly IEventBus eventBus;
        private readonly int enemyMaxHp;
        private bool hasBattleStarted;

        public CombatTurnState State { get; }

        public CombatTurnFlowService(ResolveCombatActionUseCase useCase, EnemyTurnService enemyService, CombatStatusService combatStatusService, CombatHealthService combatHealthService, IEventBus bus, int enemyHp)
        {
            resolveCombatActionUseCase = useCase;
            enemyTurnService = enemyService;
            statusService = combatStatusService;
            healthService = combatHealthService;
            eventBus = bus;
            enemyMaxHp = Mathf.Max(1, enemyHp);
            State = new CombatTurnState
            {
                TurnIndex = 0,
                Owner = TurnOwner.None,
                Phase = CombatTurnPhase.None,
            };
        }

        public void StartBattle()
        {
            if (hasBattleStarted)
            {
                Debug.LogWarning("[CombatTurnFlow] Battle already started. Restarting state.");
            }

            hasBattleStarted = true;
            statusService.ResetBattle();
            healthService.StartBattle(enemyMaxHp);
            State.TurnIndex = 1;
            State.Owner = TurnOwner.Player;
            State.Phase = CombatTurnPhase.BattleStart;
            LogState("Battle started");
            enemyTurnService.InitializeForBattle(State.TurnIndex);
            EnterPlayerTurnStart();
        }

        public void EnterPlayerTurnStart()
        {
            if (!hasBattleStarted)
            {
                Debug.LogWarning("[CombatTurnFlow] EnterPlayerTurnStart called before StartBattle.");
                return;
            }

            State.Owner = TurnOwner.Player;
            State.Phase = CombatTurnPhase.TurnStart;
            LogState("Player turn start");
            healthService.ResetPlayerBlockForTurn();
            statusService.ProcessTurnStart(CombatSide.Player, State.TurnIndex);

            State.Phase = CombatTurnPhase.Input;
            LogState("Player input phase");
        }

        public DamageContext ResolvePlayerAction(CombatActionCommand command)
        {
            if (command == null)
            {
                Debug.LogWarning("[CombatTurnFlow] ResolvePlayerAction received null command.");
                return null;
            }

            if (State.Owner != TurnOwner.Player || State.Phase != CombatTurnPhase.Input)
            {
                Debug.LogWarning($"[CombatTurnFlow] Invalid resolve request in state: {State}");
                return null;
            }

            State.Phase = CombatTurnPhase.Resolve;
            LogState("Player resolve phase");

            command.CardFlatDamageBonus += statusService.GetOutgoingFlatBonus(CombatSide.Player, command.DamageType);
            command.BoonDamageMultiplier *= statusService.GetOutgoingMultiplier(CombatSide.Player);
            command.DefenseMultiplier *= statusService.GetIncomingMultiplier(CombatSide.Enemy);
            command.TargetBlock = healthService.GetEnemyBlock();

            DamageContext result = resolveCombatActionUseCase.Execute(command);
            if (result != null)
            {
                healthService.ApplyPlayerCombatDamageToEnemy(result.FinalDamage, "PlayerResolve");
            }

            return result;
        }

        public void EndPlayerTurn()
        {
            if (State.Owner != TurnOwner.Player || (State.Phase != CombatTurnPhase.Resolve && State.Phase != CombatTurnPhase.Input))
            {
                Debug.LogWarning($"[CombatTurnFlow] Invalid EndPlayerTurn state: {State}");
                return;
            }

            State.Owner = TurnOwner.Player;
            State.Phase = CombatTurnPhase.TurnEnd;
            LogState("Player turn end");
        }

        public void EnterEnemyTurnStart()
        {
            if (State.Owner != TurnOwner.Player || State.Phase != CombatTurnPhase.TurnEnd)
            {
                Debug.LogWarning($"[CombatTurnFlow] Invalid EnterEnemyTurnStart state: {State}");
                return;
            }

            if (!healthService.IsEnemyAlive || !healthService.IsPlayerAlive)
            {
                Debug.LogWarning("[CombatTurnFlow] Enemy turn start skipped because battle already has a defeated side.");
                return;
            }

            State.Owner = TurnOwner.Enemy;
            State.Phase = CombatTurnPhase.TurnStart;
            LogState("Enemy turn start");
            healthService.ResetEnemyBlockForTurn();
            statusService.ProcessTurnStart(CombatSide.Enemy, State.TurnIndex);
        }

        public EnemyTurnResult ResolveEnemyTurn()
        {
            if (State.Owner != TurnOwner.Enemy || State.Phase != CombatTurnPhase.TurnStart)
            {
                Debug.LogWarning($"[CombatTurnFlow] Invalid enemy resolve request in state: {State}");
                return null;
            }

            State.Phase = CombatTurnPhase.Resolve;
            LogState("Enemy resolve phase");

            if (!healthService.IsEnemyAlive || !healthService.IsPlayerAlive)
            {
                Debug.LogWarning("[CombatTurnFlow] Enemy resolve skipped because one side is defeated.");
                return null;
            }

            EnemyTurnResult result = enemyTurnService.ResolveTurn(State.TurnIndex);
            return result;
        }

        public void EndEnemyTurnAndAdvance()
        {
            if (State.Owner != TurnOwner.Enemy || State.Phase != CombatTurnPhase.Resolve)
            {
                Debug.LogWarning($"[CombatTurnFlow] Invalid EndEnemyTurnAndAdvance state: {State}");
                return;
            }

            State.Owner = TurnOwner.Enemy;
            State.Phase = CombatTurnPhase.TurnEnd;
            LogState("Enemy turn end");

            State.TurnIndex += 1;
            EnterPlayerTurnStart();
        }

        private void LogState(string action)
        {
            Debug.Log($"[CombatTurnFlow] {action} | {State}");
            eventBus?.Publish(new TurnSnapshotRecorded(action, State.TurnIndex, State.Owner.ToString(), State.Phase.ToString()));
        }
    }
}
