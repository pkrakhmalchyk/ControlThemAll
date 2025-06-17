namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackForceHigherThan : IAbilityCondition
    {
        private readonly AttackForceHigherThanConfig config;
        private readonly IInputAttackContext context;


        public AttackForceHigherThan(
            AttackForceHigherThanConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;
        }


        public bool IsFulfilled()
        {
            return context.AttackInput.Force.magnitude > config.MinAttackForce;
        }
    }
}