using System.Collections.Generic;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackInRadius : IAbilityComponent
    {
        private readonly AttackInRadiusConfig config;
        private readonly ITransformContext context;
        private AttackTargetsProvider attackTargetsProvider;
        private AttackService attackService;


        public AttackInRadius(
            AttackInRadiusConfig config,
            ITransformContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(AttackTargetsProvider attackTargetsProvider, AttackService attackService)
        {
            this.attackTargetsProvider = attackTargetsProvider;
            this.attackService = attackService;
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

                foreach (IGameplayEntity target in attackTargets)
                {
                    attackService.Attack(context.Owner, target, config.Damage);
                }
            }
        }

    }
}