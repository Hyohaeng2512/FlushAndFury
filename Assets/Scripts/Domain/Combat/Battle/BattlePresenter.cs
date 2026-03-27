using FlushAndFury.Application.Combat;
using FlushAndFury.Application.Run;
using FlushAndFury.Domain.Combat;
using FlushAndFury.Infrastructure.Combat.Steps;
using FlushAndFury.Infrastructure.Events;
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
            const int maxTurns = 20;
            int safety = 0;

            while (!combatTurnFlowService.IsBattleEnded && safety < maxTurns)
            {
                DamageContext playerResult = combatTurnFlowService.ResolvePlayerAction(CreateFullDemoCommand());
                if (playerResult != null)
                {
                    Debug.Log($"[BattlePresenter] Run Turn Flow Demo | Player Final Damage: {playerResult.FinalDamage} | DamageType: {playerResult.DamageType}");
                }

                combatTurnFlowService.EndPlayerTurn();
                if (combatTurnFlowService.IsBattleEnded)
                {
                    break;
                }

                combatTurnFlowService.EnterEnemyTurnStart();
                if (combatTurnFlowService.IsBattleEnded)
                {
                    break;
                }

                EnemyTurnResult enemyResult = combatTurnFlowService.ResolveEnemyTurn();
                if (enemyResult != null)
                {
                    Debug.Log($"[BattlePresenter] Run Turn Flow Demo | Enemy Intent: {enemyResult.Intent.IntentType} | DamageToPlayer: {enemyResult.DamageToPlayer} | BlockGained: {enemyResult.BlockGained} | DebuffApplied: {enemyResult.DebuffApplied}");
                }

                if (combatTurnFlowService.IsBattleEnded)
                {
                    break;
                }

                combatTurnFlowService.EndEnemyTurnAndAdvance();
                safety += 1;
            }

            if (!combatTurnFlowService.IsBattleEnded)
            {
                Debug.LogWarning($"[BattlePresenter] Run Turn Flow Demo stopped by safety limit ({maxTurns} turns).");
            }
        }

        [ContextMenu("Run Test: PhaseShifter")]
        public void RunTestPhaseShifter()
        {
            CombatActionCommand command = new CombatActionCommand
            {
                BaseDamage = 18,
                PipelineBlock = 4,
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
                PipelineBlock = 2,
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
                PipelineBlock = 3,
                DamageType = DamageType.Physical,
                HandPattern = HandPattern.Straight,
                ActiveRelicIds = new List<string>
                {
                    CombatModifierIds.RelicPerfectFlow,
                },
            };

            ExecuteAndLog("Run Test: PerfectFlow", command);
        }

        [ContextMenu("Run Test: Pipeline Mitigation")]
        public void RunTestPipelineMitigation()
        {
            DamageContext context = new DamageContext(10, 4, DamageType.Physical);
            IDamageStep mitigationStep = new MitigationStep();
            IDamageStep finalizeStep = new FinalizeStep();

            mitigationStep.Execute(context);
            finalizeStep.Execute(context);

            bool pass = context.FinalDamage == 6;
            LogTestResult("Pipeline Mitigation", pass, $"expected=6 actual={context.FinalDamage}");
        }

        [ContextMenu("Run Test: Enemy Armor Absorb")]
        public void RunTestEnemyArmorAbsorb()
        {
            EventBus bus = new EventBus();
            RunProgressService run = new RunProgressService(bus);
            CombatHealthService health = new CombatHealthService(bus, run);

            health.StartBattle(25);
            bus.Publish(new EnemyBlockGained(5));
            health.ApplyPlayerCombatDamageToEnemy(10, "ArmorAbsorbTest");

            bool hpPass = health.EnemyCurrentHp == 20;
            bool blockPass = health.EnemyBlock == 0;
            bool pass = hpPass && blockPass;
            LogTestResult("Enemy Armor Absorb", pass, $"expectedHp=20 actualHp={health.EnemyCurrentHp} expectedBlock=0 actualBlock={health.EnemyBlock}");
        }

        [ContextMenu("Run Test: Two Layer Block")]
        public void RunTestTwoLayerBlock()
        {
            DamageContext context = new DamageContext(12, 4, DamageType.Physical);
            IDamageStep mitigationStep = new MitigationStep();
            IDamageStep finalizeStep = new FinalizeStep();
            mitigationStep.Execute(context);
            finalizeStep.Execute(context);

            EventBus bus = new EventBus();
            RunProgressService run = new RunProgressService(bus);
            CombatHealthService health = new CombatHealthService(bus, run);

            health.StartBattle(25);
            bus.Publish(new EnemyBlockGained(5));
            health.ApplyPlayerCombatDamageToEnemy(context.FinalDamage, "TwoLayerTest");

            bool pass = context.FinalDamage == 8 && health.EnemyBlock == 0 && health.EnemyCurrentHp == 22;
            LogTestResult("Two Layer Block", pass, $"pipelineExpected=8 pipelineActual={context.FinalDamage} hpExpected=22 hpActual={health.EnemyCurrentHp} blockExpected=0 blockActual={health.EnemyBlock}");
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
                PipelineBlock = 6,
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

        private void LogTestResult(string testName, bool pass, string details)
        {
            if (pass)
            {
                Debug.Log($"[BattlePresenter] TEST PASS | {testName} | {details}");
                return;
            }

            Debug.LogError($"[BattlePresenter] TEST FAIL | {testName} | {details}");
        }
    }
}
