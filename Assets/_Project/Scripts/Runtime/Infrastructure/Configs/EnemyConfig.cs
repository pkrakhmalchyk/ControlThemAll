using ControllThemAll.Runtime.Shared;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [CreateAssetMenu(menuName = "EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [ObjectNameSelector(typeof(GameObject))]
        public string PrefabName;
        public float Speed = 3f;
        public float RotationSpeed = 10f;
    }
}