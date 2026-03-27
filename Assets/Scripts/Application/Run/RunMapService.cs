using FlushAndFury.Domain.Run;
using FlushAndFury.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlushAndFury.Application.Run
{
    public sealed class RunMapService
    {
        private readonly IEventBus eventBus;
        private readonly IEncounterResolver encounterResolver;
        private readonly Dictionary<string, MapNodeState> nodes;

        private string currentNodeId;

        public bool IsMapStarted { get; private set; }
        public bool IsMapCompleted { get; private set; }
        public bool IsRunFailed { get; private set; }
        public bool IsAwaitingCombatResolution { get; private set; }

        public string CurrentNodeId => currentNodeId;

        public RunMapService(IEventBus bus, IEncounterResolver resolver)
        {
            eventBus = bus;
            encounterResolver = resolver;
            nodes = new Dictionary<string, MapNodeState>(StringComparer.OrdinalIgnoreCase);
        }

        public void StartMap()
        {
            BuildDefaultMap();

            IsMapStarted = true;
            IsMapCompleted = false;
            IsRunFailed = false;
            IsAwaitingCombatResolution = false;

            currentNodeId = "START";
            MapNodeState start = nodes[currentNodeId];
            start.IsUnlocked = true;
            start.IsCleared = true;

            UnlockOutgoingNodes(start);

            eventBus?.Publish(new MapStarted(nodes.Count, currentNodeId));
            eventBus?.Publish(new MapNodeCleared(currentNodeId, start.NodeType.ToString(), "StartNode"));
        }

        public IReadOnlyList<MapNodeState> GetAvailableNextNodes()
        {
            if (!IsMapStarted || string.IsNullOrWhiteSpace(currentNodeId))
            {
                return Array.Empty<MapNodeState>();
            }

            if (!nodes.TryGetValue(currentNodeId, out MapNodeState current))
            {
                return Array.Empty<MapNodeState>();
            }

            return current.NextNodeIds
                .Where(id => nodes.TryGetValue(id, out MapNodeState node) && node.IsUnlocked && !node.IsCleared)
                .Select(id => nodes[id])
                .ToList();
        }

        public IReadOnlyList<MapNodeState> GetAllNodes()
        {
            return nodes.Values
                .OrderBy(node => node.NodeId, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public bool TryGetNode(string nodeId, out MapNodeState node)
        {
            return nodes.TryGetValue(nodeId, out node);
        }

        public bool TryEnterNode(string nodeId)
        {
            if (!IsMapStarted || IsMapCompleted || IsRunFailed || IsAwaitingCombatResolution)
            {
                return false;
            }

            if (!nodes.TryGetValue(nodeId, out MapNodeState nextNode))
            {
                return false;
            }

            if (!nextNode.IsUnlocked || nextNode.IsCleared)
            {
                return false;
            }

            if (!CanMoveTo(nodeId))
            {
                return false;
            }

            currentNodeId = nodeId;
            eventBus?.Publish(new MapNodeEntered(nextNode.NodeId, nextNode.NodeType.ToString()));

            EncounterResolution resolution = encounterResolver.Resolve(nextNode.NodeType);
            if (resolution.AutoResolved)
            {
                ClearCurrentNode(resolution.Reason);
                return true;
            }

            if (resolution.RequiresCombat)
            {
                IsAwaitingCombatResolution = true;
                eventBus?.Publish(new MapAwaitingCombat(nextNode.NodeId, nextNode.NodeType.ToString()));
                return true;
            }

            return false;
        }

        public void ResolveCurrentCombatEncounter(bool playerWon)
        {
            if (!IsAwaitingCombatResolution || string.IsNullOrWhiteSpace(currentNodeId))
            {
                return;
            }

            if (playerWon)
            {
                ClearCurrentNode("CombatWin");
                return;
            }

            IsAwaitingCombatResolution = false;
            IsRunFailed = true;
            eventBus?.Publish(new MapRunFailed(currentNodeId, "CombatLost"));
        }

        private bool CanMoveTo(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(currentNodeId))
            {
                return false;
            }

            if (!nodes.TryGetValue(currentNodeId, out MapNodeState current))
            {
                return false;
            }

            return current.NextNodeIds.Contains(nodeId, StringComparer.OrdinalIgnoreCase);
        }

        private void ClearCurrentNode(string reason)
        {
            if (!nodes.TryGetValue(currentNodeId, out MapNodeState current))
            {
                return;
            }

            current.IsCleared = true;
            IsAwaitingCombatResolution = false;
            eventBus?.Publish(new MapNodeCleared(current.NodeId, current.NodeType.ToString(), reason));

            if (current.NodeType == MapNodeType.Boss)
            {
                IsMapCompleted = true;
                eventBus?.Publish(new MapCompleted(current.NodeId));
                return;
            }

            UnlockOutgoingNodes(current);
        }

        private void UnlockOutgoingNodes(MapNodeState source)
        {
            for (int i = 0; i < source.NextNodeIds.Count; i++)
            {
                string nextId = source.NextNodeIds[i];
                if (!nodes.TryGetValue(nextId, out MapNodeState next))
                {
                    continue;
                }

                if (next.IsUnlocked)
                {
                    continue;
                }

                next.IsUnlocked = true;
                eventBus?.Publish(new MapNodeUnlocked(next.NodeId, next.NodeType.ToString()));
            }
        }

        private void BuildDefaultMap()
        {
            nodes.Clear();

            AddNode("START", MapNodeType.Start, "C1", "E1");
            AddNode("C1", MapNodeType.Combat, "S1");
            AddNode("E1", MapNodeType.Event, "S1");
            AddNode("S1", MapNodeType.Shop, "EL1");
            AddNode("EL1", MapNodeType.Elite, "B1");
            AddNode("B1", MapNodeType.Boss);
        }

        private void AddNode(string nodeId, MapNodeType nodeType, params string[] nextNodeIds)
        {
            MapNodeState node = new MapNodeState
            {
                NodeId = nodeId,
                NodeType = nodeType,
                NextNodeIds = new List<string>(nextNodeIds ?? Array.Empty<string>()),
                IsUnlocked = false,
                IsCleared = false,
            };

            nodes[nodeId] = node;
        }
    }
}
