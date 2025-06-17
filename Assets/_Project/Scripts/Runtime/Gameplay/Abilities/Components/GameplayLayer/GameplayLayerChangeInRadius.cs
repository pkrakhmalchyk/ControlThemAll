using System.Collections.Generic;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class GameplayLayerChangeInRadius : IAbilityComponent
    {
        private readonly GameplayLayerChangeInRadiusConfig config;
        private readonly ITransformContext context;
        private AttackTargetsProvider attackTargetsProvider;
        private GameplayLayersService gameplayLayersService;


        public GameplayLayerChangeInRadius(
            GameplayLayerChangeInRadiusConfig config,
            ITransformContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(
            GameplayLayersService gameplayLayersService,
            AttackTargetsProvider attackTargetsProvider)
        {
            this.gameplayLayersService = gameplayLayersService;
            this.attackTargetsProvider = attackTargetsProvider;
        }

        public void Execute(bool execute)
        {
            if (execute)
            {
                IEnumerable<IGameplayEntity> attackTargets = attackTargetsProvider.GetAttackTargetsInSphere(
                    context.Transform.position,
                    config.Radius,
                    context.Owner.PhysicsLayer,
                    context.Owner.GameplayLayer);

                foreach (IGameplayEntity attackTarget in attackTargets)
                {
                    gameplayLayersService.SetGameplayLayer(attackTarget, context.Owner.GameplayLayer, config.Duration);
                }
            }
        }
    }
}