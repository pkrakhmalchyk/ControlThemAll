using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class CollisionInBoundsDetectedConfig : IAbilityConditionConfig<IBoundsContext>
    {
        public float BoundsMultiplier = 1f;


        public IAbilityCondition Create(IBoundsContext context)
        {
            return new CollisionInBoundsDetected(this, context);
        }
    }
}