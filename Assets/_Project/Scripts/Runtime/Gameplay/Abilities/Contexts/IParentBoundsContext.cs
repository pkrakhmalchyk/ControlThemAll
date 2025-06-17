using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public interface IParentBoundsContext : IAbilityContext
    {
        public Vector3 Bounds { get; }
        public Vector3 Position { get; }
        public Quaternion Orientation { get; }
    }
}