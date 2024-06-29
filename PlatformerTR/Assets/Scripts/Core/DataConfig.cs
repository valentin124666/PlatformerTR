using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    [CreateAssetMenu(fileName = "DataConfig", menuName = "Configs/DataConfig", order = 1)]
    public class DataConfig : ScriptableObject
    {
        [SerializeField] private PlayerData playerData;
        [SerializeField] private EnemyData enemyData;

        public PlayerData GetPlayerData() => playerData;
        public EnemyData GetEnemyData() => enemyData;
    }

    [Serializable]
    public class PlayerData
    {
        public float maxVelocityHorizontal = 12;
        public float jumpPower = 25f;
        public float maxHealth = 10;
        public float damagePower = 3f;
    }

    [Serializable]
    public class EnemyData
    {
        public float moveSpeed = 0.05f;
        public float damagePower = 1f;
        public float maxHealth = 10;
    }
}