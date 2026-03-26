using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace FlushAndFury.Application.Combat
{
    public sealed class CombatStatusService
    {
        private readonly IEventBus eventBus;
        private readonly Dictionary<CombatStatusType, CombatStatusInstance> playerStatuses;
        private readonly Dictionary<CombatStatusType, CombatStatusInstance> enemyStatuses;

        public CombatStatusService(IEventBus bus)
        {
            eventBus = bus;
            playerStatuses = new Dictionary<CombatStatusType, CombatStatusInstance>();
            enemyStatuses = new Dictionary<CombatStatusType, CombatStatusInstance>();

            eventBus?.Subscribe<ApplyStatusToPlayerRequested>(OnApplyStatusToPlayerRequested);
            eventBus?.Subscribe<ApplyStatusToTargetRequested>(OnApplyStatusToTargetRequested);
            eventBus?.Subscribe<EnemyDebuffApplied>(OnEnemyDebuffApplied);
        }

        public void ResetBattle()
        {
            playerStatuses.Clear();
            enemyStatuses.Clear();
        }

        public void ProcessTurnStart(CombatSide side, int turnIndex)
        {
            Dictionary<CombatStatusType, CombatStatusInstance> statuses = GetStatuses(side);
            List<CombatStatusType> keys = new List<CombatStatusType>(statuses.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                CombatStatusType type = keys[i];
                if (!statuses.TryGetValue(type, out CombatStatusInstance status))
                {
                    continue;
                }

                if (type == CombatStatusType.Burn && status.Stacks > 0)
                {
                    eventBus?.Publish(new CombatStatusTickDamage(side.ToString(), type.ToString(), status.Stacks, turnIndex));
                }

                if (!status.IsPermanent)
                {
                    status.RemainingTurns -= 1;
                    if (status.RemainingTurns <= 0)
                    {
                        statuses.Remove(type);
                        eventBus?.Publish(new CombatStatusExpired(side.ToString(), type.ToString()));
                    }
                }
            }
        }

        public int GetOutgoingFlatBonus(CombatSide side, DamageType damageType)
        {
            int strengthStacks = GetStacks(side, CombatStatusType.Strength);
            if (strengthStacks <= 0)
            {
                return 0;
            }

            return damageType == DamageType.Physical ? strengthStacks * 2 : strengthStacks;
        }

        public float GetOutgoingMultiplier(CombatSide side)
        {
            int weakStacks = GetStacks(side, CombatStatusType.Weak);
            if (weakStacks <= 0)
            {
                return 1f;
            }

            return 0.75f;
        }

        public float GetIncomingMultiplier(CombatSide target)
        {
            int vulnerableStacks = GetStacks(target, CombatStatusType.Vulnerable);
            if (vulnerableStacks <= 0)
            {
                return 1f;
            }

            return 1.5f;
        }

        private void OnApplyStatusToPlayerRequested(ApplyStatusToPlayerRequested signal)
        {
            ApplyById(CombatSide.Player, signal.StatusId, signal.Value, signal.DurationTurns);
        }

        private void OnApplyStatusToTargetRequested(ApplyStatusToTargetRequested signal)
        {
            int duration = GetDefaultDuration(signal.StatusId);
            ApplyById(CombatSide.Enemy, signal.StatusId, signal.Value, duration);
        }

        private void OnEnemyDebuffApplied(EnemyDebuffApplied signal)
        {
            int duration = GetDefaultDuration(signal.DebuffId);
            ApplyById(CombatSide.Player, signal.DebuffId, signal.Value, duration);
        }

        private void ApplyById(CombatSide side, string statusId, int value, int durationTurns)
        {
            if (!TryParseStatus(statusId, out CombatStatusType type))
            {
                return;
            }

            Dictionary<CombatStatusType, CombatStatusInstance> statuses = GetStatuses(side);
            if (!statuses.TryGetValue(type, out CombatStatusInstance status))
            {
                status = new CombatStatusInstance
                {
                    Type = type,
                    Stacks = 0,
                    RemainingTurns = 0,
                    IsPermanent = false,
                };
                statuses[type] = status;
            }

            status.Stacks = Math.Max(0, status.Stacks + value);

            if (durationTurns < 0)
            {
                status.IsPermanent = true;
                status.RemainingTurns = -1;
            }
            else if (!status.IsPermanent)
            {
                int normalizedDuration = Math.Max(1, durationTurns);
                status.RemainingTurns = Math.Max(status.RemainingTurns, normalizedDuration);
            }

            if (status.Stacks <= 0)
            {
                statuses.Remove(type);
                eventBus?.Publish(new CombatStatusExpired(side.ToString(), type.ToString()));
                return;
            }

            eventBus?.Publish(new CombatStatusApplied(side.ToString(), type.ToString(), status.Stacks, status.RemainingTurns, status.IsPermanent));
        }

        private Dictionary<CombatStatusType, CombatStatusInstance> GetStatuses(CombatSide side)
        {
            return side == CombatSide.Player ? playerStatuses : enemyStatuses;
        }

        private int GetStacks(CombatSide side, CombatStatusType type)
        {
            Dictionary<CombatStatusType, CombatStatusInstance> statuses = GetStatuses(side);
            if (statuses.TryGetValue(type, out CombatStatusInstance status))
            {
                return status.Stacks;
            }

            return 0;
        }

        private bool TryParseStatus(string rawId, out CombatStatusType statusType)
        {
            if (string.Equals(rawId, "Strength", StringComparison.OrdinalIgnoreCase))
            {
                statusType = CombatStatusType.Strength;
                return true;
            }

            if (string.Equals(rawId, "Burn", StringComparison.OrdinalIgnoreCase))
            {
                statusType = CombatStatusType.Burn;
                return true;
            }

            if (string.Equals(rawId, "Vulnerable", StringComparison.OrdinalIgnoreCase))
            {
                statusType = CombatStatusType.Vulnerable;
                return true;
            }

            if (string.Equals(rawId, "Weak", StringComparison.OrdinalIgnoreCase))
            {
                statusType = CombatStatusType.Weak;
                return true;
            }

            statusType = CombatStatusType.Strength;
            return false;
        }

        private int GetDefaultDuration(string statusId)
        {
            if (string.Equals(statusId, "Burn", StringComparison.OrdinalIgnoreCase)) return 3;
            if (string.Equals(statusId, "Weak", StringComparison.OrdinalIgnoreCase)) return 2;
            if (string.Equals(statusId, "Vulnerable", StringComparison.OrdinalIgnoreCase)) return 2;
            if (string.Equals(statusId, "Strength", StringComparison.OrdinalIgnoreCase)) return -1;

            return 1;
        }
    }
}
