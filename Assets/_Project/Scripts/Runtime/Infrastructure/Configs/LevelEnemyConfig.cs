using System;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [Serializable]
    public class LevelEnemyConfig
    {
        public string EnemyId;
        public string WeaponId;
        public string GameplayLayer;
        public Vector3 SpawnPoint;
        public float MaxHealth;
        public float Health;
    }
}