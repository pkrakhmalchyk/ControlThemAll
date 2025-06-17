using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class GameplayLayerChangeInRadiusConfig : IAbilityComponentConfig<ITransformContext>
    {
        public float Duration;
        public float Radius;

        public IAbilityComponent Create(ITransformContext context)
        {
            return new GameplayLayerChangeInRadius(this, context);
        }
    }
}