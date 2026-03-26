using FlushAndFury.Application.Combat;
using FlushAndFury.Domain.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace FlushAndFury.Presentation.Battle
{
    public class BattlePresenter : MonoBehaviour
    {
        private ResolveCombatActionUseCase resolveCombatActionUseCase;
        private CombatTurnFlowService combatTurnFlowService;

        public void SetUseCase(ResolveCombatActionUseCase useCase)
        {
            resolveCombatActionUseCase = useCase;
        }

        public void SetTurnFlowService(CombatTurnFlowService turnFlowService)
        {
            combatTurnFlowService = turnFlowService;
        }

        [ContextMenu("Run Combat Demo")]
        public void RunCombatDemo()
        {
            ExecuteAndLog("Run Combat Demo", CreateFullDemoCommand());
        }

        [ContextMenu("Run Turn Flow Demo")]
        public void RunTurnFlowDemo()
        {
            if (combatTurnFlowService == null)
            {
                Debug.LogWarning("[BattlePresenter] CombatTurnFlowService is not bound yet.");
                return;
            }

            combatTurnFlowService.StartBattle();
            DamageContext playerResult = combatTurnFlowService.ResolvePlayerAction(CreateFullDemoCommand());
            if (playerResult != null)
            {
                Debug.Log($"[BattlePresenter] Run Turn Flow Demo | Player Final Damage: {playerResult.FinalDamage} | DamageType: {playerResult.DamageType}");
            }

            combatTurnFlowService.EndPlayerTurn();
            combatTurnFlowService.EnterEnemyTurnStart();
            EnemyTurnResult enemyResult = combatTurnFlowService.ResolveEnemyTurn();
            if (enemyResult != null)
            {
                Debug.Log($"[BattlePresenter] Run Turn Flow Demo | Enemy Intent: {enemyResult.Intent.IntentType} | DamageToPlayer: {enemyResult.DamageToPlayer} | BlockGained: {enemyResult.BlockGained} | DebuffApplied: {enemyResult.DebuffApplied}");
            }
            combatTurnFlowService.EndEnemyTurnAndAdvance();
        }

        [ContextMenu("Run Test: PhaseShifter")]
        public void RunTestPhaseShifter()
        {
            CombatActionCommand command = new CombatActionCommand
            {
                BaseDamage = 18,
                TargetBlock = 4,
                DamageType = DamageType.Magic,
                EnemyDefenseRule = EnemyDefenseRule.PhaseShifter,
                EnemyPhaseMask = EnemyPhaseMask.PhysicalOnly,
            };

            ExecuteAndLog("Run Test: PhaseShifter", command);
        }

        [ContextMenu("Run Test: ShieldGuardian")]
        public void RunTestShieldGuardian()
        {
            CombatActionCommand command = new CombatActionCommand
            {
                BaseDamage = 16,
                TargetBlock = 2,
                DamageType = DamageType.Physical,
                EnemyDefenseRule = EnemyDefenseRule.ShieldGuardian,
                HitsTakenThisTurn = 0,
            };

            ExecuteAndLog("Run Test: ShieldGuardian", command);
        }

        [ContextMenu("Run Test: PerfectFlow")]
        public void RunTestPerfectFlow()
        {
            CombatActionCommand command = new CombatActionCommand
            {
                BaseDamage = 12,
                TargetBlock = 3,
                DamageType = DamageType.Physical,
                HandPattern = HandPattern.Straight,
                ActiveRelicIds = new List<string>
                {
                    CombatModifierIds.RelicPerfectFlow,
                },
            };

            ExecuteAndLog("Run Test: PerfectFlow", command);
        }

        private void ExecuteAndLog(string label, CombatActionCommand command)
        {
            if (resolveCombatActionUseCase == null)
            {
                Debug.LogWarning("[BattlePresenter] ResolveCombatActionUseCase is not bound yet.");
                return;
            }

            DamageContext result = resolveCombatActionUseCase.Execute(command);
            Debug.Log($"[BattlePresenter] {label} | Final Damage: {result.FinalDamage} | DamageType: {result.DamageType}");
        }

        private CombatActionCommand CreateFullDemoCommand()
        {
            return new CombatActionCommand
            {
                BaseDamage = 14,
                CardFlatDamageBonus = 0,
                RelicFlatDamageBonus = 0,
                RelicDamageMultiplier = 1f,
                TargetBlock = 6,
                DamageType = DamageType.Physical,
                HandPattern = HandPattern.Pair,
                DiscardCountThisTurn = 1,
                HpLostThisTurn = 1,
                EnemyDefenseRule = EnemyDefenseRule.MirrorBeast,
                ReflectThreshold = 10,
                PlayedCardEnchants = new List<string>
                {
                    CombatModifierIds.EnchantFlat3,
                    CombatModifierIds.EnchantEnergy1,
                    CombatModifierIds.EnchantBurn2,
                    CombatModifierIds.EnchantConvertToMagic,
                },
                PlayedCardTags = new List<string>
                {
                    CombatModifierIds.TagWarrior,
                    CombatModifierIds.TagArcane,
                    CombatModifierIds.TagLucky,
                },
                PlayedCardSeals = new List<string>
                {
                    CombatModifierIds.SealReturn,
                    CombatModifierIds.SealGoldOnKill,
                    CombatModifierIds.SealTopDeck,
                },
                ActiveRelicIds = new List<string>
                {
                    CombatModifierIds.RelicWarriorEmblem,
                    CombatModifierIds.RelicArcaneCore,
                    CombatModifierIds.RelicBloodPact,
                    CombatModifierIds.RelicGamblersCoin,
                },
            };
        }
    }
}
