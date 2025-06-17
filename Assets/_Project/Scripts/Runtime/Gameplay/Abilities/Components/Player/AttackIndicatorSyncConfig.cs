using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackIndicatorSyncConfig : IAbilityComponentConfig<PlayerWeaponContext>
    {
        public float MinAttackForce;

        public IAbilityComponent Create(PlayerWeaponContext context)
        {
            return new AttackIndicatorSync(this, context);
        }
    }
}