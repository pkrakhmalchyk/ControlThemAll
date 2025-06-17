using System.Collections.Generic;
using ControllThemAll.Runtime.Shared;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [CreateAssetMenu(menuName = "EnemyWeaponConfig/EnemyWeaponConfig")]
    public class EnemyWeaponConfig : ScriptableObject
    {
        public WeaponConfig WeaponConfig;
        [ObjectNameSelector(typeof(EnemyWeaponAbilityConfig))]
        public List<string> AbilitiesIds;
    }
}
