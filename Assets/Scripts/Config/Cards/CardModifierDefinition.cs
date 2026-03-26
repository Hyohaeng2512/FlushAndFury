using UnityEngine;

namespace FlushAndFury.Config.Cards
{
    [CreateAssetMenu(menuName = "FlushAndFury/Config/Card Modifier", fileName = "CardModifierDefinition")]
    public class CardModifierDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
    }
}
