using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class ConstantDirectionalMovementInput : IMovementInput
    {
        private Vector3 direction;


        public Vector3 Direction => direction;


        public ConstantDirectionalMovementInput(Vector3 direction)
        {
            this.direction = direction;
        }
    }
}