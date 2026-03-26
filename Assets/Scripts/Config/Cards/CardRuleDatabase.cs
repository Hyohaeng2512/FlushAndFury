using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlushAndFury.Config.Cards
{
    [CreateAssetMenu(menuName = "FlushAndFury/Config/Card Rule Database", fileName = "CardRuleDatabase")]
    public class CardRuleDatabase : ScriptableObject
    {
        [SerializeField] private List<CardRuleDefinition> rules = new List<CardRuleDefinition>();

        public List<CardRuleDefinition> GetRules(IEnumerable<string> ids, CardRuleCategory category)
        {
            if (ids == null)
            {
                return new List<CardRuleDefinition>();
            }

            HashSet<string> idSet = new HashSet<string>(ids.Where(id => !string.IsNullOrWhiteSpace(id)), StringComparer.OrdinalIgnoreCase);
            return rules
                .Where(rule => rule != null && rule.Category == category && idSet.Contains(rule.Id))
                .OrderBy(rule => rule.Priority)
                .ToList();
        }

        public bool Contains(string id, CardRuleCategory category)
        {
            return rules.Any(rule => rule != null && rule.Category == category && string.Equals(rule.Id, id, StringComparison.OrdinalIgnoreCase));
        }
    }
}
