using System.Collections.Generic;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackInParentBounds : IAbilityComponent
    {
        private AttackInParentBoundsConfig config;
        private IParentBoundsContext context;
        private AttackTargetsProvider attackTargetsProvider;
        private AttackService attackService;

        public AttackInParentBounds(
            AttackInParentBoundsConfig config,
            IParentBoundsContext context)
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
                IEnumerable<IGameplayEntity> attackTargets = attackTargetsProvider.GetAttackTargetsInBox(
                    context.Position,
                    context.Bounds * config.BoundsMultiplier,
                    context.Orientation,
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