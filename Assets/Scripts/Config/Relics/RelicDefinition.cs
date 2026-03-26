using UnityEngine;

namespace FlushAndFury.Config.Relics
{
    [CreateAssetMenu(menuName = "FlushAndFury/Config/Relic", fileName = "RelicDefinition")]
    public class RelicDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
    }
}
