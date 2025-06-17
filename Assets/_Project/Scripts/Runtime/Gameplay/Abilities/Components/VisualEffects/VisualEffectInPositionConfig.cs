using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class VisualEffectInPositionConfig : IAbilityComponentConfig<ITransformContext>
    {
        public VfxType Type;
        public float Radius;


        public IAbilityComponent Create(ITransformContext context)
        {
            return new VisualEffectInPosition(this, context);
        }
    }
}