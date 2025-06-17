using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class CollisionInParentBoundsDetectedConfig : IAbilityConditionConfig<IParentBoundsContext>
    {
        public float BoundsMultiplier = 1f;


        public IAbilityCondition Create(IParentBoundsContext context)
        {
            return new CollisionInParentBoundsDetected(this, context);
        }
    }
}