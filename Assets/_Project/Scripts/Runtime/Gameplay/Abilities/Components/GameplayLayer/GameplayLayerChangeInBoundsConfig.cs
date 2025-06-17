using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class GameplayLayerChangeInBoundsConfig : IAbilityComponentConfig<IBoundsContext>
    {
        public float Duration;
        public float BoundsMultiplier = 1f;

        public IAbilityComponent Create(IBoundsContext context)
        {
            return new GameplayLayerChangeInBounds(this, context);
        }
    }
}