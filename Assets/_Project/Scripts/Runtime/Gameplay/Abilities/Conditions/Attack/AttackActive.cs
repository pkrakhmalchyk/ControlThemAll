namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackActive : IAbilityCondition
    {
        private readonly AttackActiveConfig config;
        private readonly IInputAttackContext context;


        public AttackActive(
            AttackActiveConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;
        }


        public bool IsFulfilled()
        {
            return context.AttackInput.IsActive;
        }
    }
}