using System;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AttackForceInDirectionConfig : IAbilityConditionConfig<IInputAttackContext>
    {
        public Vector3 AttackDirection;


        public IAbilityCondition Create(IInputAttackContext context)
        {
            return new AttackForceInDirection(this, context);
        }
    }
}