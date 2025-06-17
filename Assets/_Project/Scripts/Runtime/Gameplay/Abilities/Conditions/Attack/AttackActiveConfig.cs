using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackActiveConfig : IAbilityConditionConfig<IInputAttackContext>
    {
        public IAbilityCondition Create(IInputAttackContext context)
        {
            return new AttackActive(this, context);
        }
    }
}