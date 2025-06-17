using System;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [Serializable]
    public class LevelPlayerConfig
    {
        public string GameplayLayer;
        public Vector3 SpawnPoint;
        public float MaxHealth;
        public float Health;
    }
}