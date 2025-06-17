using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class PlayerWeaponContext : IInputAttackContext
    {
        private readonly WeaponView weapon;
        private readonly IAttackInput attackInput;
        private readonly PlayerView player;


        public IGameplayEntity Owner => weapon;
        public IAttackInput AttackInput => attackInput;
        public Vector3 ParticleSpawnPoint => player.ParticleSpawnPoint.position;
        public PlayerView Player => player;


        public PlayerWeaponContext(
            WeaponView weapon,
            PlayerView player,
            IAttackInput attackInput)
        {
            this.weapon = weapon;
            this.player = player;
            this.attackInput = attackInput;
        }

    }
}