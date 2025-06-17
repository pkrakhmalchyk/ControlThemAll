using System;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [Serializable]
    public class LevelBrickConfig
    {
        public string GameplayLayer;
        public Vector3 Position;
        public Vector3 Scale;
    }
}