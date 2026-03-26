using FlushAndFury.Domain.Combat;
using UnityEngine;

namespace FlushAndFury.Application.Combat
{
    public sealed class CombatTurnFlowService
    {
        private readonly ResolveCombatActionUseCase resolveCombatActionUseCase;

        public CombatTurnState State { get; }

        public CombatTurnFlowService(ResolveCombatActionUseCase useCase)
        {
            resolveCombatActionUseCase = useCase;
            State = new CombatTurnState
            {
                TurnIndex = 0,
                Owner = TurnOwner.None,
                Phase = CombatTurnPhase.None,
            };
        }

        public void StartBattle()
        {
            State.TurnIndex = 1;
            State.Owner = TurnOwner.Player;
            State.Phase = CombatTurnPhase.BattleStart;
            LogState("Battle started");
            EnterPlayerTurnStart();
        }

        public void EnterPlayerTurnStart()
        {
            State.Owner = TurnOwner.Player;
            State.Phase = CombatTurnPhase.TurnStart;
            LogState("Player turn start");

            State.Phase = CombatTurnPhase.Input;
            LogState("Player input phase");
        }

        public DamageContext ResolvePlayerAction(CombatActionCommand command)
        {
            if (State.Owner != TurnOwner.Player || State.Phase != CombatTurnPhase.Input)
            {
                Debug.LogWarning($"[CombatTurnFlow] Invalid resolve request in state: {State}");
                return null;
            }

            State.Phase = CombatTurnPhase.Resolve;
            LogState("Player resolve phase");

            DamageContext result = resolveCombatActionUseCase.Execute(command);
            return result;
        }

        public void EndPlayerTurn()
        {
            State.Owner = TurnOwner.Player;
            State.Phase = CombatTurnPhase.TurnEnd;
            LogState("Player turn end");
        }

        public void EnterEnemyTurnStart()
        {
            State.Owner = TurnOwner.Enemy;
            State.Phase = CombatTurnPhase.TurnStart;
            LogState("Enemy turn start");
        }

        public void ResolveEnemyTurnNoop()
        {
            if (State.Owner != TurnOwner.Enemy || State.Phase != CombatTurnPhase.TurnStart)
            {
                Debug.LogWarning($"[CombatTurnFlow] Invalid enemy resolve request in state: {State}");
                return;
            }

            State.Phase = CombatTurnPhase.Resolve;
            LogState("Enemy resolve phase (noop)");
        }

        public void EndEnemyTurnAndAdvance()
        {
            State.Owner = TurnOwner.Enemy;
            State.Phase = CombatTurnPhase.TurnEnd;
            LogState("Enemy turn end");

            State.TurnIndex += 1;
            EnterPlayerTurnStart();
        }

        private void LogState(string action)
        {
            Debug.Log($"[CombatTurnFlow] {action} | {State}");
        }
    }
}
