using FlushAndFury.Domain.Combat;
using UnityEngine;

namespace FlushAndFury.Config.Relics
{
    public enum RelicRuleActionType
    {
        ApplyPlayerStatus = 0,
        ModifyDamageMultiplier = 1,
        DealRandomEnemyDamage = 2,
        DrawCard = 3,
    }

    [CreateAssetMenu(menuName = "FlushAndFury/Config/Relic Rule", fileName = "RelicRuleDefinition")]
    public class RelicRuleDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private int priority = 100;
        [SerializeField] private RelicRuleActionType actionType;

        [Header("Conditions")]
        [SerializeField] private bool requireMagicDamage;
        [SerializeField] private bool requireHpLostThisTurn;
        [SerializeField] private bool requireDiscardThisTurn;
        [SerializeField] private HandPattern requiredHandPattern = HandPattern.HighCard;
        [SerializeField] private bool useHandPattern;
        [SerializeField] private string requiredTagId;

        [Header("Payload")]
        [SerializeField] private int intValue;
        [SerializeField] private float floatValue = 1f;
        [SerializeField] private string stringValue;
        [SerializeField] private int durationTurns = 1;

        public string Id => id;
        public int Priority => priority;
        public RelicRuleActionType ActionType => actionType;
        public bool RequireMagicDamage => requireMagicDamage;
        public bool RequireHpLostThisTurn => requireHpLostThisTurn;
        public bool RequireDiscardThisTurn => requireDiscardThisTurn;
        public HandPattern RequiredHandPattern => requiredHandPattern;
        public bool UseHandPattern => useHandPattern;
        public string RequiredTagId => requiredTagId;
        public int IntValue => intValue;
        public float FloatValue => floatValue;
        public string StringValue => stringValue;
        public int DurationTurns => durationTurns;
    }
}
