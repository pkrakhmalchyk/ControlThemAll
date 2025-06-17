using System;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Infrastructure;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class StraightInputShootConfig : IAbilityComponentConfig<IInputAttackContext>
    {
        [ObjectNameSelector(typeof(ParticleConfig))]
        public string ParticleId;


        public IAbilityComponent Create(IInputAttackContext context)
        {
            return new StraightInputShoot(this, context);
        }
    }
}