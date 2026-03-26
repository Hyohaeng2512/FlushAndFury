using FlushAndFury.Infrastructure.Events;
using UnityEngine;

namespace FlushAndFury.Presentation.Battle
{
    public class CombatDebugListener : MonoBehaviour
    {
        private IEventBus eventBus;
        private bool isSubscribed;

        public void Bind(IEventBus bus)
        {
            if (eventBus == bus)
            {
                return;
            }

            Unsubscribe();
            eventBus = bus;
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (eventBus == null || isSubscribed)
            {
                return;
            }

            eventBus.Subscribe<EnergyGainRequested>(OnEnergyGainRequested);
            eventBus.Subscribe<ApplyStatusToTargetRequested>(OnApplyStatusToTargetRequested);
            eventBus.Subscribe<LuckyProcCheckRequested>(OnLuckyProcCheckRequested);
            eventBus.Subscribe<CardReturnToDeckRequested>(OnCardReturnToDeckRequested);
            eventBus.Subscribe<GoldOnKillFlagRequested>(OnGoldOnKillFlagRequested);
            eventBus.Subscribe<CardPlaceTopDeckRequested>(OnCardPlaceTopDeckRequested);
            eventBus.Subscribe<ApplyStatusToPlayerRequested>(OnApplyStatusToPlayerRequested);
            eventBus.Subscribe<DealRandomEnemyDamageRequested>(OnDealRandomEnemyDamageRequested);
            eventBus.Subscribe<DrawCardRequested>(OnDrawCardRequested);
            eventBus.Subscribe<ReflectDamageToPlayerRequested>(OnReflectDamageToPlayerRequested);
            eventBus.Subscribe<EnemyGainBuffRequested>(OnEnemyGainBuffRequested);
            eventBus.Subscribe<EnemyChargeIfPlayerSkippedAttackRequested>(OnEnemyChargeIfPlayerSkippedAttackRequested);

            isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (eventBus == null || !isSubscribed)
            {
                return;
            }

            eventBus.Unsubscribe<EnergyGainRequested>(OnEnergyGainRequested);
            eventBus.Unsubscribe<ApplyStatusToTargetRequested>(OnApplyStatusToTargetRequested);
            eventBus.Unsubscribe<LuckyProcCheckRequested>(OnLuckyProcCheckRequested);
            eventBus.Unsubscribe<CardReturnToDeckRequested>(OnCardReturnToDeckRequested);
            eventBus.Unsubscribe<GoldOnKillFlagRequested>(OnGoldOnKillFlagRequested);
            eventBus.Unsubscribe<CardPlaceTopDeckRequested>(OnCardPlaceTopDeckRequested);
            eventBus.Unsubscribe<ApplyStatusToPlayerRequested>(OnApplyStatusToPlayerRequested);
            eventBus.Unsubscribe<DealRandomEnemyDamageRequested>(OnDealRandomEnemyDamageRequested);
            eventBus.Unsubscribe<DrawCardRequested>(OnDrawCardRequested);
            eventBus.Unsubscribe<ReflectDamageToPlayerRequested>(OnReflectDamageToPlayerRequested);
            eventBus.Unsubscribe<EnemyGainBuffRequested>(OnEnemyGainBuffRequested);
            eventBus.Unsubscribe<EnemyChargeIfPlayerSkippedAttackRequested>(OnEnemyChargeIfPlayerSkippedAttackRequested);

            isSubscribed = false;
        }

        private void OnEnergyGainRequested(EnergyGainRequested signal)
        {
            Debug.Log($"[CombatDebugListener] EnergyGainRequested amount={signal.Amount}");
        }

        private void OnApplyStatusToTargetRequested(ApplyStatusToTargetRequested signal)
        {
            Debug.Log($"[CombatDebugListener] ApplyStatusToTargetRequested status={signal.StatusId} value={signal.Value}");
        }

        private void OnLuckyProcCheckRequested(LuckyProcCheckRequested signal)
        {
            Debug.Log("[CombatDebugListener] LuckyProcCheckRequested");
        }

        private void OnCardReturnToDeckRequested(CardReturnToDeckRequested signal)
        {
            Debug.Log("[CombatDebugListener] CardReturnToDeckRequested");
        }

        private void OnGoldOnKillFlagRequested(GoldOnKillFlagRequested signal)
        {
            Debug.Log("[CombatDebugListener] GoldOnKillFlagRequested");
        }

        private void OnCardPlaceTopDeckRequested(CardPlaceTopDeckRequested signal)
        {
            Debug.Log("[CombatDebugListener] CardPlaceTopDeckRequested");
        }

        private void OnApplyStatusToPlayerRequested(ApplyStatusToPlayerRequested signal)
        {
            Debug.Log($"[CombatDebugListener] ApplyStatusToPlayerRequested status={signal.StatusId} value={signal.Value} duration={signal.DurationTurns}");
        }

        private void OnDealRandomEnemyDamageRequested(DealRandomEnemyDamageRequested signal)
        {
            Debug.Log($"[CombatDebugListener] DealRandomEnemyDamageRequested amount={signal.Amount}");
        }

        private void OnDrawCardRequested(DrawCardRequested signal)
        {
            Debug.Log($"[CombatDebugListener] DrawCardRequested amount={signal.Amount}");
        }

        private void OnReflectDamageToPlayerRequested(ReflectDamageToPlayerRequested signal)
        {
            Debug.Log($"[CombatDebugListener] ReflectDamageToPlayerRequested amount={signal.Amount}");
        }

        private void OnEnemyGainBuffRequested(EnemyGainBuffRequested signal)
        {
            Debug.Log($"[CombatDebugListener] EnemyGainBuffRequested buff={signal.BuffId} value={signal.Value}");
        }

        private void OnEnemyChargeIfPlayerSkippedAttackRequested(EnemyChargeIfPlayerSkippedAttackRequested signal)
        {
            Debug.Log("[CombatDebugListener] EnemyChargeIfPlayerSkippedAttackRequested");
        }
    }
}
