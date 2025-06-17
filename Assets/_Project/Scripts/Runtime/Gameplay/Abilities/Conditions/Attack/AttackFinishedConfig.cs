using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackFinishedConfig : IAbilityConditionConfig<IInputAttackContext>
    {
        public IAbilityCondition Create(IInputAttackContext context)
        {
            return new AttackFinished(this, context);
        }
    }
}