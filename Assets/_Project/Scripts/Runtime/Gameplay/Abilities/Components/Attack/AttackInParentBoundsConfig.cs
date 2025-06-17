using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackInParentBoundsConfig : IAbilityComponentConfig<IParentBoundsContext>
    {

        public Damage Damage;
        public float BoundsMultiplier = 1f;


        public IAbilityComponent Create(IParentBoundsContext context)
        {
            return new AttackInParentBounds(this, context);
        }
    }
}