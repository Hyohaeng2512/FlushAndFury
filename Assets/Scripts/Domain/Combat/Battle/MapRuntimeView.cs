using FlushAndFury.Application.Run;
using FlushAndFury.Domain.Run;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlushAndFury.Presentation.Battle
{
    public sealed class MapRuntimeView : MonoBehaviour
    {
        [SerializeField] private BattlePresenter battlePresenter;
        [SerializeField] private float nodeSize = 52f;
        [SerializeField] private bool autoResolveCombatWhenAwaiting;

        private RunMapService mapService;
        private bool isBound;

        private readonly Dictionary<string, Vector2> nodePositions = new Dictionary<string, Vector2>(StringComparer.OrdinalIgnoreCase);
        private readonly List<(string from, string to)> edges = new List<(string from, string to)>();

        private static Texture2D lineTexture;

        public void Bind(RunMapService service)
        {
            mapService = service;
            isBound = mapService != null;
            RebuildLayout();
        }

        private void Awake()
        {
            if (battlePresenter == null)
            {
                battlePresenter = FindAnyObjectByType<BattlePresenter>();
            }

            EnsureLineTexture();
        }

        private void OnGUI()
        {
            if (battlePresenter == null)
            {
                return;
            }

            Rect panel = new Rect(452, 16, 520, 420);
            GUI.Box(panel, "Map View");

            if (!isBound)
            {
                GUI.Label(new Rect(panel.x + 14, panel.y + 30, 400, 20), "Map service not bound.");
                return;
            }

            if (!mapService.IsMapStarted)
            {
                if (GUI.Button(new Rect(panel.x + 14, panel.y + 30, 120, 28), "Start Map"))
                {
                    if (battlePresenter.StartMapSession())
                    {
                        RebuildLayout();
                    }
                }

                return;
            }

            if (GUI.Button(new Rect(panel.x + 14, panel.y + 30, 120, 28), "Restart Map"))
            {
                if (battlePresenter.StartMapSession())
                {
                    RebuildLayout();
                }
            }

            GUI.Label(new Rect(panel.x + 146, panel.y + 34, 350, 20), $"Current={mapService.CurrentNodeId} AwaitCombat={mapService.IsAwaitingCombatResolution}");

            Rect mapRect = new Rect(panel.x + 12, panel.y + 66, panel.width - 24, panel.height - 78);
            GUI.Box(mapRect, string.Empty);

            DrawEdges(mapRect);
            DrawNodes(mapRect);

            if (mapService.IsAwaitingCombatResolution)
            {
                if (autoResolveCombatWhenAwaiting)
                {
                    battlePresenter.ResolvePendingMapCombat();
                }
                else if (GUI.Button(new Rect(panel.x + panel.width - 180, panel.y + 30, 160, 28), "Resolve Combat"))
                {
                    battlePresenter.ResolvePendingMapCombat();
                }
            }
        }

        private void DrawEdges(Rect mapRect)
        {
            Color previous = GUI.color;
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            for (int i = 0; i < edges.Count; i++)
            {
                (string from, string to) edge = edges[i];
                if (!nodePositions.TryGetValue(edge.from, out Vector2 fromPos) || !nodePositions.TryGetValue(edge.to, out Vector2 toPos))
                {
                    continue;
                }

                Vector2 p1 = new Vector2(mapRect.x + fromPos.x, mapRect.y + fromPos.y);
                Vector2 p2 = new Vector2(mapRect.x + toPos.x, mapRect.y + toPos.y);
                DrawLine(p1, p2, 2f);
            }

            GUI.color = previous;
        }

        private void DrawNodes(Rect mapRect)
        {
            IReadOnlyList<MapNodeState> nodes = mapService.GetAllNodes();
            for (int i = 0; i < nodes.Count; i++)
            {
                MapNodeState node = nodes[i];
                if (!nodePositions.TryGetValue(node.NodeId, out Vector2 pos))
                {
                    continue;
                }

                Rect buttonRect = new Rect(
                    mapRect.x + pos.x - (nodeSize * 0.5f),
                    mapRect.y + pos.y - (nodeSize * 0.5f),
                    nodeSize,
                    nodeSize
                );

                Color previous = GUI.color;
                GUI.color = GetNodeColor(node);

                string label = GetNodeLabel(node);
                bool clicked = GUI.Button(buttonRect, label);

                GUI.color = previous;

                if (clicked)
                {
                    battlePresenter.TryEnterMapNode(node.NodeId);
                }
            }
        }

        private Color GetNodeColor(MapNodeState node)
        {
            if (string.Equals(node.NodeId, mapService.CurrentNodeId, StringComparison.OrdinalIgnoreCase))
            {
                return new Color(1f, 0.95f, 0.5f, 1f);
            }

            if (node.IsCleared)
            {
                return new Color(0.45f, 0.85f, 0.45f, 1f);
            }

            if (node.IsUnlocked)
            {
                return new Color(0.85f, 0.85f, 0.95f, 1f);
            }

            return new Color(0.35f, 0.35f, 0.35f, 1f);
        }

        private static string GetNodeLabel(MapNodeState node)
        {
            switch (node.NodeType)
            {
                case MapNodeType.Start: return "S";
                case MapNodeType.Combat: return "C";
                case MapNodeType.Elite: return "E";
                case MapNodeType.Shop: return "$";
                case MapNodeType.Event: return "?";
                case MapNodeType.Boss: return "B";
                default: return node.NodeType.ToString();
            }
        }

        private void RebuildLayout()
        {
            nodePositions.Clear();
            edges.Clear();

            if (!isBound)
            {
                return;
            }

            IReadOnlyList<MapNodeState> nodes = mapService.GetAllNodes();
            if (nodes.Count == 0)
            {
                return;
            }

            Dictionary<string, int> depthByNode = BuildDepthMap(nodes);
            int maxDepth = depthByNode.Values.DefaultIfEmpty(0).Max();
            float width = 460f;
            float height = 320f;
            float xStep = maxDepth <= 0 ? 0f : width / maxDepth;

            for (int depth = 0; depth <= maxDepth; depth++)
            {
                List<MapNodeState> row = nodes
                    .Where(node => depthByNode.TryGetValue(node.NodeId, out int d) && d == depth)
                    .OrderBy(node => node.NodeId, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (row.Count == 0)
                {
                    continue;
                }

                float yStep = height / (row.Count + 1);
                for (int i = 0; i < row.Count; i++)
                {
                    MapNodeState node = row[i];
                    float x = 26f + (depth * xStep);
                    float y = 16f + ((i + 1) * yStep);
                    nodePositions[node.NodeId] = new Vector2(x, y);
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                MapNodeState node = nodes[i];
                for (int j = 0; j < node.NextNodeIds.Count; j++)
                {
                    edges.Add((node.NodeId, node.NextNodeIds[j]));
                }
            }
        }

        private static Dictionary<string, int> BuildDepthMap(IReadOnlyList<MapNodeState> nodes)
        {
            Dictionary<string, MapNodeState> lookup = nodes.ToDictionary(node => node.NodeId, StringComparer.OrdinalIgnoreCase);
            Dictionary<string, int> depths = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            Queue<string> queue = new Queue<string>();

            if (lookup.ContainsKey("START"))
            {
                depths["START"] = 0;
                queue.Enqueue("START");
            }
            else if (nodes.Count > 0)
            {
                depths[nodes[0].NodeId] = 0;
                queue.Enqueue(nodes[0].NodeId);
            }

            while (queue.Count > 0)
            {
                string currentId = queue.Dequeue();
                if (!lookup.TryGetValue(currentId, out MapNodeState node))
                {
                    continue;
                }

                int currentDepth = depths[currentId];
                for (int i = 0; i < node.NextNodeIds.Count; i++)
                {
                    string nextId = node.NextNodeIds[i];
                    int nextDepth = currentDepth + 1;

                    if (!depths.TryGetValue(nextId, out int oldDepth) || nextDepth > oldDepth)
                    {
                        depths[nextId] = nextDepth;
                        queue.Enqueue(nextId);
                    }
                }
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                if (!depths.ContainsKey(nodes[i].NodeId))
                {
                    depths[nodes[i].NodeId] = 0;
                }
            }

            return depths;
        }

        private static void EnsureLineTexture()
        {
            if (lineTexture != null)
            {
                return;
            }

            lineTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            lineTexture.SetPixel(0, 0, Color.white);
            lineTexture.Apply();
        }

        private static void DrawLine(Vector2 p1, Vector2 p2, float thickness)
        {
            Vector2 delta = p2 - p1;
            float length = delta.magnitude;
            if (length <= 0.001f)
            {
                return;
            }

            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            Matrix4x4 previous = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, p1);
            GUI.DrawTexture(new Rect(p1.x, p1.y - (thickness * 0.5f), length, thickness), lineTexture);
            GUI.matrix = previous;
        }
    }
}
