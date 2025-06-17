using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public interface IInputAttackContext : IAbilityContext
    {
        public Vector3 ParticleSpawnPoint { get; }
        public IAttackInput AttackInput { get; }
    }
}