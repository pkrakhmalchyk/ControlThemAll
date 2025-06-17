using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackSelfConfig : IAbilityComponentConfig<IAbilityContext>
    {
        public AbilityTarget AttackTarget;
        public Damage Damage;


        public IAbilityComponent Create(IAbilityContext context)
        {
            return new AttackSelf(this, context);
        }
    }
}