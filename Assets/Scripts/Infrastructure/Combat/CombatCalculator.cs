using FlushAndFury.Domain.Combat;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlushAndFury.Infrastructure.Combat
{
    public sealed class CombatCalculator : ICombatCalculator
    {
        private readonly IReadOnlyList<IDamageStep> steps;

        public CombatCalculator(IEnumerable<IDamageStep> damageSteps)
        {
            steps = damageSteps.OrderBy(step => step.Order).ToList();
        }

        public DamageContext Resolve(DamageContext context)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                IDamageStep step = steps[i];
                step.Execute(context);

                Debug.Log($"[CombatCalculator] Step: {step.GetType().Name} | DamageType: {context.DamageType} | CurrentDamage: {context.CurrentDamage:0.##}");
            }

            return context;
        }
    }
}
