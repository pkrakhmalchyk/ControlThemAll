using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public interface ITouchInput
    {
        public bool IsActive { get; }
        public Vector3 Position { get; }
    }
}