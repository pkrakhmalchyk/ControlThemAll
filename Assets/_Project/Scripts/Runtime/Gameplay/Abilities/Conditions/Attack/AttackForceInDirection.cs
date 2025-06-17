using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackForceInDirection : IAbilityCondition
    {
        private readonly AttackForceInDirectionConfig config;
        private readonly IInputAttackContext context;


        public AttackForceInDirection(
            AttackForceInDirectionConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;
        }


        public bool IsFulfilled()
        {
            return Vector3.Dot(context.AttackInput.Force, config.AttackDirection) > 0
                || context.AttackInput.Force == Vector3.zero;
        }
    }
}