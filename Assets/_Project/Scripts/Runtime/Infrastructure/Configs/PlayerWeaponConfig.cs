using System.Collections.Generic;
using ControllThemAll.Runtime.Shared;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [CreateAssetMenu(menuName = "PlayerWeaponConfig/PlayerWeaponConfig")]
    public class PlayerWeaponConfig : ScriptableObject
    {
        [Range(0f, 2f)]
        public float InputSensitivity = 1f;
        [Range(5f, 180f)]
        public float MaxAimHorizontalAngle = 25f;
        public WeaponConfig WeaponConfig;
        [ObjectNameSelector(typeof(PlayerWeaponAbilityConfig))]
        public List<string> AbilitiesIds;
        public Sprite Icon;
    }
}
