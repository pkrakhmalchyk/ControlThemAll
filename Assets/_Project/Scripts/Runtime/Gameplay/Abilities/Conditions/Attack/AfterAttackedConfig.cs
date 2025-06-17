using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AfterAttackedConfig : IAbilityConditionConfig<IAbilityContext>
    {
        public AbilityTarget AttackTarget;
        public float AttackedStateDuration = 1f;

        public IAbilityCondition Create(IAbilityContext context)
        {
            return new AfterAttacked(this, context);
        }
    }
}