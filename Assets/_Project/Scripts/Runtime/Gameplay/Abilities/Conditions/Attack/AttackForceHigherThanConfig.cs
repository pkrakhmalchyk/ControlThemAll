using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackForceHigherThanConfig : IAbilityConditionConfig<IInputAttackContext>
    {
        public float MinAttackForce;


        public IAbilityCondition Create(IInputAttackContext context)
        {
            return new AttackForceHigherThan(this, context);
        }
    }
}