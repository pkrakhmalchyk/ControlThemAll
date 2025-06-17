using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackSelf : IAbilityComponent
    {
        private AttackSelfConfig config;
        private IAbilityContext context;
        private AttackService attackService;

        public AttackSelf(
            AttackSelfConfig config,
            IAbilityContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(AttackService attackService)
        {
            this.attackService = attackService;
        }


        public void Execute(bool execute)
        {
            if (execute)
            {
                if (config.AttackTarget == AbilityTarget.Self)
                {
                    attackService.Attack(context.Owner, context.Owner, config.Damage);
                }
                else if (config.AttackTarget == AbilityTarget.Parent)
                {
                    attackService.Attack(context.Owner.Parent, context.Owner.Parent, config.Damage);
                }
            }
        }
    }
}