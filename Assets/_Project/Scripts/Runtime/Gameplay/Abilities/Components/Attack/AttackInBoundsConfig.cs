using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackInBoundsConfig : IAbilityComponentConfig<IBoundsContext>
    {
        public Damage Damage;
        public float BoundsMultiplier = 1f;


        public IAbilityComponent Create(IBoundsContext context)
        {
            return new AttackInBounds(this, context);
        }
    }
}