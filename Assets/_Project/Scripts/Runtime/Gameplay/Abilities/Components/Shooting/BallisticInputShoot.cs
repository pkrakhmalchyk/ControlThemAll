using System.Collections.Generic;
using ControllThemAll.Runtime.Infrastructure;
using UnityEngine;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class BallisticInputShoot : IAbilityComponent
    {
        private readonly BallisticInputShootConfig config;
        private readonly IInputAttackContext context;
        private ShootingService shootingService;
        private ParticlesConfigsContainer particlesConfigsContainer;


        public BallisticInputShoot(
            BallisticInputShootConfig config,
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
            Vector3 force = context.AttackInput.Force.normalized
                            * Mathf.Clamp(context.AttackInput.Force.magnitude, config.MinAttackForce, config.MaxAttackForce);

            IEnumerable<Vector3> trajectoryPoints = TrajectoryPredictor.PredictBallisticTrajectory(
                context.ParticleSpawnPoint,
                force,
                float.MaxValue);

            shootingService.ShootAlongTrajectory(
                particlesConfigsContainer.GetParticleConfig(config.ParticleId),
                context.Owner,
                trajectoryPoints,
                context.ParticleSpawnPoint);
        }
    }
}