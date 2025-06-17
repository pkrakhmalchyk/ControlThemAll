using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public interface IAttackInput
    {
        public bool IsActive { get; }
        public Vector3 Force { get; }
    }
}