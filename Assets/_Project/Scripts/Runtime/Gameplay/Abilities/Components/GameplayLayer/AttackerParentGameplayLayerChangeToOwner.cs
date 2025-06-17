using System;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackerParentGameplayLayerChangeToOwner : IAbilityComponent, IDisposable
    {
        private readonly AttackerParentGameplayLayerChangeToOwnerConfig config;
        private readonly IAbilityContext context;
        private AttackService attackService;
        private GameplayLayersService gameplayLayersService;


        private IGameplayEntity lastAttacker;


        public AttackerParentGameplayLayerChangeToOwner(
            AttackerParentGameplayLayerChangeToOwnerConfig config,
            IAbilityContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(
            AttackService attackService,
            GameplayLayersService gameplayLayersService)
        {
            this.attackService = attackService;
            this.gameplayLayersService = gameplayLayersService;

            this.attackService.Attacked += OnAttacked;
        }

        public void Execute(bool execute)
        {
            if (!execute || lastAttacker?.Parent == null)
            {
                return;
            }

            if (lastAttacker.Parent.GameplayLayer != context.Owner.GameplayLayer)
            {
                gameplayLayersService.SetGameplayLayer(lastAttacker.Parent, context.Owner.GameplayLayer, config.Duration);
            }
        }

        public void Dispose()
        {
            attackService.Attacked -= OnAttacked;
        }


        private void OnAttacked(IGameplayEntity attacker, IGameplayEntity target, float damage)
        {
            if (target.Id == (context.Owner.Parent?.Id ?? context.Owner.Id))
            {
                lastAttacker = attacker;
            }
        }
    }
}