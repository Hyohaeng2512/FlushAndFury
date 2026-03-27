using FlushAndFury.Application.Run;
using FlushAndFury.Infrastructure.Events;
using UnityEngine;

namespace FlushAndFury.Application.Combat
{
    public sealed class CombatHealthService
    {
        private readonly IEventBus eventBus;
        private readonly RunProgressService runProgressService;

        public int EnemyMaxHp { get; private set; }
        public int EnemyCurrentHp { get; private set; }
        public int PlayerBlock { get; private set; }
        public int EnemyBlock { get; private set; }

        public bool IsPlayerAlive => runProgressService.CurrentHp > 0;
        public bool IsEnemyAlive => EnemyCurrentHp > 0;

        public CombatHealthService(IEventBus bus, RunProgressService runState)
        {
            eventBus = bus;
            runProgressService = runState;

            eventBus?.Subscribe<EnemyAttackResolved>(OnEnemyAttackResolved);
            eventBus?.Subscribe<CombatStatusTickDamage>(OnCombatStatusTickDamage);
            eventBus?.Subscribe<DealRandomEnemyDamageRequested>(OnDealRandomEnemyDamageRequested);
            eventBus?.Subscribe<EnemyBlockGained>(OnEnemyBlockGained);
            eventBus?.Subscribe<ReflectDamageToPlayerRequested>(OnReflectDamageToPlayerRequested);
        }

        public void StartBattle(int enemyMaxHp)
        {
            EnemyMaxHp = Mathf.Max(1, enemyMaxHp);
            EnemyCurrentHp = EnemyMaxHp;
            PlayerBlock = 0;
            EnemyBlock = 0;

            eventBus?.Publish(new CombatHealthChanged("Enemy", EnemyCurrentHp, EnemyMaxHp, 0, "BattleStart"));
            eventBus?.Publish(new CombatHealthChanged("Player", runProgressService.CurrentHp, runProgressService.MaxHp, 0, "BattleStart"));
            eventBus?.Publish(new CombatBlockChanged("Player", PlayerBlock, "BattleStart"));
            eventBus?.Publish(new CombatBlockChanged("Enemy", EnemyBlock, "BattleStart"));
        }

        public void ResetPlayerBlockForTurn()
        {
            if (PlayerBlock == 0) return;
            PlayerBlock = 0;
            eventBus?.Publish(new CombatBlockChanged("Player", PlayerBlock, "TurnStartPlayer"));
        }

        public void ResetEnemyBlockForTurn()
        {
            if (EnemyBlock == 0) return;
            EnemyBlock = 0;
            eventBus?.Publish(new CombatBlockChanged("Enemy", EnemyBlock, "TurnStartEnemy"));
        }

        public int GetEnemyBlock()
        {
            return EnemyBlock;
        }

        public void ApplyPlayerCombatDamageToEnemy(int damage, string reason)
        {
            ApplyDamageToEnemy(damage, reason);
        }

        private void OnEnemyAttackResolved(EnemyAttackResolved signal)
        {
            ApplyDamageToPlayer(signal.Damage, "EnemyAttackResolved");
        }

        private void OnCombatStatusTickDamage(CombatStatusTickDamage signal)
        {
            if (signal.Side == "Enemy")
            {
                ApplyDamageToEnemy(signal.Damage, "StatusTick");
                return;
            }

            if (signal.Side == "Player")
            {
                ApplyDamageToPlayer(signal.Damage, "StatusTick");
            }
        }

        private void OnDealRandomEnemyDamageRequested(DealRandomEnemyDamageRequested signal)
        {
            ApplyDamageToEnemy(signal.Amount, "DealRandomEnemyDamageRequested");
        }

        private void OnEnemyBlockGained(EnemyBlockGained signal)
        {
            EnemyBlock = Mathf.Max(0, EnemyBlock + signal.Amount);
            eventBus?.Publish(new CombatBlockChanged("Enemy", EnemyBlock, "EnemyBlockGained"));
        }

        private void OnReflectDamageToPlayerRequested(ReflectDamageToPlayerRequested signal)
        {
            ApplyDamageToPlayer(signal.Amount, "ReflectDamage");
        }

        private void ApplyDamageToEnemy(int incomingDamage, string reason)
        {
            int damage = Mathf.Max(0, incomingDamage);
            if (damage <= 0 || !IsEnemyAlive)
            {
                return;
            }

            int blocked = Mathf.Min(EnemyBlock, damage);
            EnemyBlock -= blocked;
            int hpDamage = damage - blocked;

            if (blocked > 0)
            {
                eventBus?.Publish(new CombatBlockChanged("Enemy", EnemyBlock, reason + "_BlockAbsorb"));
            }

            if (hpDamage <= 0)
            {
                return;
            }

            EnemyCurrentHp = Mathf.Max(0, EnemyCurrentHp - hpDamage);
            eventBus?.Publish(new CombatHealthChanged("Enemy", EnemyCurrentHp, EnemyMaxHp, hpDamage, reason));

            if (EnemyCurrentHp <= 0)
            {
                eventBus?.Publish(new CombatEntityDefeated("Enemy", reason));
            }
        }

        private void ApplyDamageToPlayer(int incomingDamage, string reason)
        {
            int damage = Mathf.Max(0, incomingDamage);
            if (damage <= 0 || !IsPlayerAlive)
            {
                return;
            }

            int blocked = Mathf.Min(PlayerBlock, damage);
            PlayerBlock -= blocked;
            int hpDamage = damage - blocked;

            if (blocked > 0)
            {
                eventBus?.Publish(new CombatBlockChanged("Player", PlayerBlock, reason + "_BlockAbsorb"));
            }

            if (hpDamage <= 0)
            {
                return;
            }

            int nextHp = Mathf.Max(0, runProgressService.CurrentHp - hpDamage);
            runProgressService.SetCurrentHp(nextHp, reason);
            eventBus?.Publish(new CombatHealthChanged("Player", runProgressService.CurrentHp, runProgressService.MaxHp, hpDamage, reason));

            if (runProgressService.CurrentHp <= 0)
            {
                eventBus?.Publish(new CombatEntityDefeated("Player", reason));
            }
        }
    }
}
