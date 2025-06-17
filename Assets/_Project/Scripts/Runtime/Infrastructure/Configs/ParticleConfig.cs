using ControllThemAll.Runtime.Shared;
using System.Collections.Generic;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [CreateAssetMenu(menuName = "ParticleConfig/ParticleConfig")]
    public class ParticleConfig : ScriptableObject
    {
        [ObjectNameSelector(typeof(GameObject))]
        public string PrefabName;
        public float Speed = 30f;
        public float LifeTime = 3f;
        [ObjectNameSelector(typeof(ParticleAbilityConfig))]
        public List<string> AbilitiesIds;
    }
}