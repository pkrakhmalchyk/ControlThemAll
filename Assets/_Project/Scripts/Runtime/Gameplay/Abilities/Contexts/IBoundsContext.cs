using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public interface IBoundsContext : IAbilityContext
    {
        public Vector3 Bounds { get; }
        public Vector3 Position { get; }
        public Quaternion Orientation { get; }
    }
}