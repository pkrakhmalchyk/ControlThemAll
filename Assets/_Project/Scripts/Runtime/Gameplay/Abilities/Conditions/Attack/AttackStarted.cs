namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackStarted : IAbilityCondition
    {
        private readonly AttackStartedConfig config;
        private readonly IInputAttackContext context;

        private bool hasAttackStarted = false;


        public AttackStarted(
            AttackStartedConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;
        }


        public bool IsFulfilled()
        {
            if (hasAttackStarted && !context.AttackInput.IsActive)
            {
                hasAttackStarted = false;
            }

            if (!hasAttackStarted && context.AttackInput.IsActive)
            {
                hasAttackStarted = true;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}