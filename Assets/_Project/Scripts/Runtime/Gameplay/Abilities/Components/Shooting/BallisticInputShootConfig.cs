using System;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Infrastructure;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class BallisticInputShootConfig : IAbilityComponentConfig<IInputAttackContext>
    {
        [ObjectNameSelector(typeof(ParticleConfig))]
        public string ParticleId;
        public float MinAttackForce = 10f;
        public float MaxAttackForce = 30f;

        public IAbilityComponent Create(IInputAttackContext context)
        {
            return new BallisticInputShoot(this, context);
        }
    }
}