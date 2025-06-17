using ControllThemAll.Runtime.Infrastructure;
using UnityEngine;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class StraightInputShoot : IAbilityComponent
    {
        private readonly StraightInputShootConfig config;
        private readonly IInputAttackContext context;
        private ShootingService shootingService;
        private ParticlesConfigsContainer particlesConfigsContainer;


        public StraightInputShoot(
            StraightInputShootConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(
            ShootingService shootingService,
            ParticlesConfigsContainer particlesConfigsContainer)
        {
            this.shootingService = shootingService;
            this.particlesConfigsContainer = particlesConfigsContainer;
        }


        public void Execute(bool execute)
        {
            if (execute)
            {
                Shoot();
            }
        }


        private void Shoot()
        {
            Vector3 forceProjection = new Vector3(context.AttackInput.Force.x, 0, context.AttackInput.Force.z);

            shootingService.ShootStraight(
                particlesConfigsContainer.GetParticleConfig(config.ParticleId),
                context.Owner,
                forceProjection.normalized,
                context.ParticleSpawnPoint);
        }
    }
}