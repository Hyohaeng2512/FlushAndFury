using UnityEngine;

namespace FlushAndFury.Config.Cards
{
    public enum CardRuleCategory
    {
        Enchant = 0,
        Tag = 1,
        Seal = 2,
    }

    public enum CardRuleEffectType
    {
        FlatDamageBonus = 0,
        DamageMultiplier = 1,
        ConvertToMagic = 2,
        PublishEnergyGain = 3,
        PublishApplyStatusToTarget = 4,
        PublishLuckyProc = 5,
        PublishCardReturnToDeck = 6,
        PublishGoldOnKillFlag = 7,
        PublishCardPlaceTopDeck = 8,
    }

    [CreateAssetMenu(menuName = "FlushAndFury/Config/Card Rule", fileName = "CardRuleDefinition")]
    public class CardRuleDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private CardRuleCategory category;
        [SerializeField] private int priority = 100;
        [SerializeField] private CardRuleEffectType effectType;
        [SerializeField] private int intValue;
        [SerializeField] private float floatValue = 1f;
        [SerializeField] private string stringValue;

        public string Id => id;
        public CardRuleCategory Category => category;
        public int Priority => priority;
        public CardRuleEffectType EffectType => effectType;
        public int IntValue => intValue;
        public float FloatValue => floatValue;
        public string StringValue => stringValue;
    }
}
