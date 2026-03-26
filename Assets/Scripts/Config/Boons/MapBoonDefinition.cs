using UnityEngine;

namespace FlushAndFury.Config.Boons
{
    [CreateAssetMenu(menuName = "FlushAndFury/Config/Map Boon", fileName = "MapBoonDefinition")]
    public class MapBoonDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
    }
}
