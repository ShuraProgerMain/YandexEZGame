using System;
using Enemy;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class BaseParams
    {
        public float heath;
        public float moveSpeed;
        public float damage;
        public float attackSpeed;
        public float attackRange;
    }
    
    [CreateAssetMenu(menuName = "Bebra/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        public BaseParams baseParams;
        public EnemyEntity prefab;
    }
}