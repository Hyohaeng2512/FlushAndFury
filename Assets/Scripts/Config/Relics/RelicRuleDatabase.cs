using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlushAndFury.Config.Relics
{
    [CreateAssetMenu(menuName = "FlushAndFury/Config/Relic Rule Database", fileName = "RelicRuleDatabase")]
    public class RelicRuleDatabase : ScriptableObject
    {
        [SerializeField] private List<RelicRuleDefinition> rules = new List<RelicRuleDefinition>();

        public List<RelicRuleDefinition> GetRules(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return new List<RelicRuleDefinition>();
            }

            HashSet<string> idSet = new HashSet<string>(ids.Where(id => !string.IsNullOrWhiteSpace(id)), StringComparer.OrdinalIgnoreCase);
            return rules
                .Where(rule => rule != null && idSet.Contains(rule.Id))
                .OrderBy(rule => rule.Priority)
                .ToList();
        }
    }
}
