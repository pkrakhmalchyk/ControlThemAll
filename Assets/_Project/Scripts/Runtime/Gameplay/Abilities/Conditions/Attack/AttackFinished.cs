namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackFinished : IAbilityCondition
    {
        private readonly AttackFinishedConfig config;
        private readonly IInputAttackContext context;

        private bool hasAttackStarted;


        public AttackFinished(
            AttackFinishedConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;
        }


        public bool IsFulfilled()
        {
            if (!hasAttackStarted && context.AttackInput.IsActive)
            {
                hasAttackStarted = true;
            }

            if (hasAttackStarted && !context.AttackInput.IsActive)
            {
                hasAttackStarted = false;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}