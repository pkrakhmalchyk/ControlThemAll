using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class StraightInputAimConfig : IAbilityComponentConfig<IInputAttackContext>
    {
        public float MaxRange = 10f;


        public IAbilityComponent Create(IInputAttackContext context)
        {
            return new StraightInputAim(this, context);
        }
    }
}