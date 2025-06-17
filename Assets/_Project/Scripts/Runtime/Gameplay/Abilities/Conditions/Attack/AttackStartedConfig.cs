using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackStartedConfig : IAbilityConditionConfig<IInputAttackContext>
    {
        public IAbilityCondition Create(IInputAttackContext context)
        {
            return new AttackStarted(this, context);
        }
    }
}