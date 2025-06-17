using ControllThemAll.Runtime.Infrastructure;
using UnityEngine;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class VisualEffectInPosition : IAbilityComponent
    {
        private readonly VisualEffectInPositionConfig config;
        private readonly ITransformContext context;
        private VfxService vfxService;
        private GameplayLayersConfigsContainer gameplayLayersConfigsContainer;


        public VisualEffectInPosition(
            VisualEffectInPositionConfig config,
            ITransformContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(
            VfxService vfxService,
            GameplayLayersConfigsContainer gameplayLayersConfigsContainer)
        {
            this.vfxService = vfxService;
            this.gameplayLayersConfigsContainer = gameplayLayersConfigsContainer;
        }

        public void Execute(bool execute)
        {
            if (execute)
            {
                Color color = gameplayLayersConfigsContainer.GetEntityGameplayLayerMainColor(context.Owner);

                if (config.Radius <= 0)
                {
                    vfxService.PlayEffect(config.Type, context.Transform.position, color);
                }
                else
                {
                    vfxService.PlayEffect(config.Type, context.Transform.position, color, config.Radius);
                }
            }
        }
    }
}