using FlushAndFury.Application.Combat;
using FlushAndFury.Application.Run;
using FlushAndFury.Domain.Run;
using FlushAndFury.Infrastructure.Events;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FlushAndFury.Presentation.Battle
{
    public sealed class CombatRuntimeHud : MonoBehaviour
    {
        private IEventBus eventBus;
        private CombatTurnFlowService turnFlowService;
        private RunMapService mapService;

        private bool isBound;

        private int playerHp;
        private int playerMaxHp;
        private int playerBlock;
        private int enemyHp;
        private int enemyMaxHp;
        private int enemyBlock;
        private int gold;

        private string enemyIntent = "-";
        private string lastBattleResult = "-";
        private string lastMapEvent = "-";

        private readonly StringBuilder hudBuilder = new StringBuilder(512);

        public void Bind(IEventBus bus, CombatTurnFlowService flowService, RunMapService runMapService)
        {
            if (eventBus == bus && turnFlowService == flowService && mapService == runMapService && isBound)
            {
                return;
            }

            Unbind();

            eventBus = bus;
            turnFlowService = flowService;
            mapService = runMapService;

            if (eventBus == null)
            {
                return;
            }

            eventBus.Subscribe<RunStateChanged>(OnRunStateChanged);
            eventBus.Subscribe<CombatHealthChanged>(OnCombatHealthChanged);
            eventBus.Subscribe<CombatBlockChanged>(OnCombatBlockChanged);
            eventBus.Subscribe<EnemyIntentTelegraphed>(OnEnemyIntentTelegraphed);
            eventBus.Subscribe<EnemyIntentSelected>(OnEnemyIntentSelected);
            eventBus.Subscribe<BattleEnded>(OnBattleEnded);
            eventBus.Subscribe<StageCleared>(OnStageCleared);
            eventBus.Subscribe<MapStarted>(OnMapStarted);
            eventBus.Subscribe<MapNodeEntered>(OnMapNodeEntered);
            eventBus.Subscribe<MapNodeCleared>(OnMapNodeCleared);
            eventBus.Subscribe<MapRunFailed>(OnMapRunFailed);
            eventBus.Subscribe<MapCompleted>(OnMapCompleted);

            isBound = true;
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Unbind()
        {
            if (!isBound || eventBus == null)
            {
                eventBus = null;
                turnFlowService = null;
                mapService = null;
                isBound = false;
                return;
            }

            eventBus.Unsubscribe<RunStateChanged>(OnRunStateChanged);
            eventBus.Unsubscribe<CombatHealthChanged>(OnCombatHealthChanged);
            eventBus.Unsubscribe<CombatBlockChanged>(OnCombatBlockChanged);
            eventBus.Unsubscribe<EnemyIntentTelegraphed>(OnEnemyIntentTelegraphed);
            eventBus.Unsubscribe<EnemyIntentSelected>(OnEnemyIntentSelected);
            eventBus.Unsubscribe<BattleEnded>(OnBattleEnded);
            eventBus.Unsubscribe<StageCleared>(OnStageCleared);
            eventBus.Unsubscribe<MapStarted>(OnMapStarted);
            eventBus.Unsubscribe<MapNodeEntered>(OnMapNodeEntered);
            eventBus.Unsubscribe<MapNodeCleared>(OnMapNodeCleared);
            eventBus.Unsubscribe<MapRunFailed>(OnMapRunFailed);
            eventBus.Unsubscribe<MapCompleted>(OnMapCompleted);

            eventBus = null;
            turnFlowService = null;
            mapService = null;
            isBound = false;
        }

        private void OnGUI()
        {
            if (!isBound)
            {
                return;
            }

            GUI.Box(new Rect(16, 16, 420, 320), "Flush & Fury Runtime HUD");

            hudBuilder.Clear();
            hudBuilder.AppendLine($"Run: HP {playerHp}/{playerMaxHp} | Gold {gold}");
            hudBuilder.AppendLine($"Combat: PlayerBlock {playerBlock} | EnemyHP {enemyHp}/{enemyMaxHp} | EnemyBlock {enemyBlock}");

            if (turnFlowService != null)
            {
                hudBuilder.AppendLine($"Turn: {turnFlowService.State.TurnIndex} | Owner {turnFlowService.State.Owner} | Phase {turnFlowService.State.Phase}");
            }
            else
            {
                hudBuilder.AppendLine("Turn: (service not bound)");
            }

            if (mapService != null)
            {
                hudBuilder.AppendLine($"Map: Node {mapService.CurrentNodeId} | AwaitCombat {mapService.IsAwaitingCombatResolution} | Completed {mapService.IsMapCompleted} | Failed {mapService.IsRunFailed}");

                IReadOnlyList<MapNodeState> availableNodes = mapService.GetAvailableNextNodes();
                if (availableNodes.Count > 0)
                {
                    hudBuilder.Append("Next: ");
                    for (int i = 0; i < availableNodes.Count; i++)
                    {
                        MapNodeState node = availableNodes[i];
                        hudBuilder.Append(node.NodeId).Append('(').Append(node.NodeType).Append(')');
                        if (i < availableNodes.Count - 1)
                        {
                            hudBuilder.Append(", ");
                        }
                    }

                    hudBuilder.AppendLine();
                }
                else
                {
                    hudBuilder.AppendLine("Next: -");
                }
            }
            else
            {
                hudBuilder.AppendLine("Map: (service not bound)");
            }

            hudBuilder.AppendLine($"Enemy Intent: {enemyIntent}");
            hudBuilder.AppendLine($"Battle Result: {lastBattleResult}");
            hudBuilder.AppendLine($"Last Map Event: {lastMapEvent}");

            GUI.Label(new Rect(28, 44, 396, 280), hudBuilder.ToString());
        }

        private void OnRunStateChanged(RunStateChanged signal)
        {
            playerHp = signal.CurrentHp;
            playerMaxHp = signal.MaxHp;
            gold = signal.Gold;
        }

        private void OnCombatHealthChanged(CombatHealthChanged signal)
        {
            if (signal.Side == "Player")
            {
                playerHp = signal.CurrentHp;
                playerMaxHp = signal.MaxHp;
                return;
            }

            if (signal.Side == "Enemy")
            {
                enemyHp = signal.CurrentHp;
                enemyMaxHp = signal.MaxHp;
            }
        }

        private void OnCombatBlockChanged(CombatBlockChanged signal)
        {
            if (signal.Side == "Player")
            {
                playerBlock = signal.CurrentBlock;
                return;
            }

            if (signal.Side == "Enemy")
            {
                enemyBlock = signal.CurrentBlock;
            }
        }

        private void OnEnemyIntentTelegraphed(EnemyIntentTelegraphed signal)
        {
            enemyIntent = $"{signal.IntentType} ({signal.Value})";
        }

        private void OnEnemyIntentSelected(EnemyIntentSelected signal)
        {
            enemyIntent = $"{signal.IntentType} ({signal.Value})";
        }

        private void OnBattleEnded(BattleEnded signal)
        {
            lastBattleResult = $"Winner={signal.Winner} Turn={signal.TurnIndex}";
        }

        private void OnStageCleared(StageCleared signal)
        {
            lastMapEvent = $"StageCleared {signal.StageIndex}, +{signal.RewardGold} gold";
        }

        private void OnMapStarted(MapStarted signal)
        {
            lastMapEvent = $"MapStarted (nodes={signal.NodeCount})";
        }

        private void OnMapNodeEntered(MapNodeEntered signal)
        {
            lastMapEvent = $"Entered {signal.NodeId} ({signal.NodeType})";
        }

        private void OnMapNodeCleared(MapNodeCleared signal)
        {
            lastMapEvent = $"Cleared {signal.NodeId} ({signal.NodeType})";
        }

        private void OnMapRunFailed(MapRunFailed signal)
        {
            lastMapEvent = $"RunFailed at {signal.NodeId} ({signal.Reason})";
        }

        private void OnMapCompleted(MapCompleted signal)
        {
            lastMapEvent = $"MapCompleted at {signal.BossNodeId}";
        }
    }
}
