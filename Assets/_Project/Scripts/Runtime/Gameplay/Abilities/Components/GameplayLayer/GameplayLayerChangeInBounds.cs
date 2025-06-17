using System.Collections.Generic;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class GameplayLayerChangeInBounds : IAbilityComponent
    {
        private readonly GameplayLayerChangeInBoundsConfig config;
        private readonly IBoundsContext context;
        private GameplayLayersService gameplayLayersService;
        private AttackTargetsProvider attackTargetsProvider;


        public GameplayLayerChangeInBounds(
            GameplayLayerChangeInBoundsConfig config,
            IBoundsContext context)
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
                IEnumerable<IGameplayEntity> attackTargets = attackTargetsProvider.GetAttackTargetsInBox(
                    context.Position,
                    context.Bounds * config.BoundsMultiplier,
                    context.Orientation,
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