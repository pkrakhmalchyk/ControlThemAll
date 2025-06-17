using System;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackerParentGameplayLayerChangeToOwnerConfig : IAbilityComponentConfig<IAbilityContext>
    {
        public float Duration;

        public IAbilityComponent Create(IAbilityContext context)
        {
            return new AttackerParentGameplayLayerChangeToOwner(this, context);
        }
    }
}