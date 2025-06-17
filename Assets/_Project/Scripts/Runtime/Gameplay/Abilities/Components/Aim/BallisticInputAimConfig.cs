using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class BallisticInputAimConfig : IAbilityComponentConfig<IInputAttackContext>
    {
        public float MaxRange = 50f;
        public float MinAttackForce = 10f;
        public float MaxAttackForce = 30f;
        public float AimAreaRadius = 3f;


        public IAbilityComponent Create(IInputAttackContext context)
        {
            return new BallisticInputAim(this, context);
        }
    }
}