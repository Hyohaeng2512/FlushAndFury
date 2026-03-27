using System.Collections.Generic;

namespace FlushAndFury.Domain.Run
{
    public sealed class MapNodeState
    {
        public string NodeId { get; set; }
        public MapNodeType NodeType { get; set; }
        public List<string> NextNodeIds { get; set; } = new List<string>();
        public bool IsUnlocked { get; set; }
        public bool IsCleared { get; set; }
    }
}
