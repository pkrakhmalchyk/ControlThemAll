using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public interface ITransformContext : IAbilityContext
    {
        public Transform Transform { get; }
    }
}