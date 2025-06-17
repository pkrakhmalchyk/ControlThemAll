using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackInRadiusConfig : IAbilityComponentConfig<ITransformContext>
    {
        public Damage Damage;
        public float Radius;

        public IAbilityComponent Create(ITransformContext context)
        {
            return new AttackInRadius(this, context);
        }
    }
}