using FlushAndFury.Domain.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace FlushAndFury.Config.Enemies
{
    [System.Serializable]
    public class EnemyIntentOption
    {
        public EnemyIntentType intentType = EnemyIntentType.Attack;
        public int minValue = 1;
        public int maxValue = 1;
        public int weight = 1;
        public string description = "Action";
    }

    [CreateAssetMenu(menuName = "FlushAndFury/Config/Enemy Intent Profile", fileName = "EnemyIntentProfile")]
    public class EnemyIntentProfile : ScriptableObject
    {
        [SerializeField] private List<EnemyIntentOption> options = new List<EnemyIntentOption>();

        public List<EnemyIntentOption> Options => options;
    }
}
