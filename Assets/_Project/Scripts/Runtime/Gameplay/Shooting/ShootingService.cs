using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class ShootingService
    {
        public Action<IGameplayEntity> Fired;


        private readonly ParticleFactory particleFactory;
        private readonly AbilitiesService abilitiesService;
        private readonly CollisionDetectionService collisionDetectionService;
        private readonly TimeScaleHolder timeScaleHolder;
        private readonly GameplayLayersConfigsContainer gameplayLayersConfigsContainer;
        private readonly ParticlesConfigsContainer particlesConfigsContainer;

        private GameObject particlesParentGo;


        public ShootingService(
            ParticleFactory particleFactory,
            AbilitiesService abilitiesService,
            CollisionDetectionService collisionDetectionService,
            TimeScaleHolder timeScaleHolder,
            GameplayLayersConfigsContainer gameplayLayersConfigsContainer,
            ParticlesConfigsContainer particlesConfigsContainer)
        {
            this.particleFactory = particleFactory;
            this.abilitiesService = abilitiesService;
            this.collisionDetectionService = collisionDetectionService;
            this.timeScaleHolder = timeScaleHolder;
            this.gameplayLayersConfigsContainer = gameplayLayersConfigsContainer;
            this.particlesConfigsContainer = particlesConfigsContainer;

            particlesParentGo = new GameObject("Particles");
        }


        public void ShootStraight(ParticleConfig particleConfig, IGameplayEntity owner, Vector3 direction, Vector3 particleSpawnPoint)
        {
            string ownerLayerName = LayerMask.LayerToName(owner.PhysicsLayer);
            bool isPlayer = RuntimeConstants.PhysicLayers.PlayerLayers.Contains(ownerLayerName);
            ParticleView particle = particleFactory.CreateParticle(particleConfig, isPlayer);
            ConstantDirectionalMovementInput movementInput = new ConstantDirectionalMovementInput(direction.normalized);

            particle.SetMovementInput(movementInput);

            ShootInternal(particle, particleConfig, owner, particleSpawnPoint);
        }

        public void ShootAlongTrajectory(ParticleConfig particleConfig, IGameplayEntity owner, IEnumerable<Vector3> trajectory, Vector3 particleSpawnPoint)
        {
            string ownerLayerName = LayerMask.LayerToName(owner.PhysicsLayer);
            bool isPlayer = RuntimeConstants.PhysicLayers.PlayerLayers.Contains(ownerLayerName);
            ParticleView particle = particleFactory.CreateParticle(particleConfig, isPlayer);
            ConstantAlongTrajectoryMovementInput movementInput = new ConstantAlongTrajectoryMovementInput(trajectory, particle.gameObject.transform);

            particle.SetMovementInput(movementInput);

            ShootInternal(particle, particleConfig, owner, particleSpawnPoint);
        }


        private void ShootInternal(ParticleView particle, ParticleConfig particleConfig, IGameplayEntity owner, Vector3 particleSpawnPoint)
        {
            particle.gameObject.transform.position = particleSpawnPoint;
            particle.transform.SetParent(particlesParentGo.transform);

            IGameplayEntity parent = owner.Parent ?? owner;
            DefaultGameplayLayerConfig defaultGameplayLayerConfig =
                gameplayLayersConfigsContainer.GetDefaultGameplayLayerConfig(parent.GameplayLayer);

            particle.SetParent(parent);
            particle.SetGameplayLayer(owner.GameplayLayer);
            particle.SetColor(defaultGameplayLayerConfig.Color);

            ParticleContext particleContext = new ParticleContext(particle);
            IEnumerable<AbilityConfig<ParticleContext>> abilitiesConfigs = particleConfig.AbilitiesIds
                .Select(id => particlesConfigsContainer.GetParticleAbilityConfig(id));

            abilitiesService.BindAbilities(abilitiesConfigs, particleContext);
            abilitiesService.SetAbilitiesActive(particle.Id, true);

            particle.gameObject.SetActive(true);

            DestroyWithDelayOrAfterCollision(particle, particleConfig.LifeTime).Forget();
            Fired?.Invoke(owner);
        }

        private async UniTask DestroyWithDelayOrAfterCollision(ParticleView particle, float delay)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            int particleId = particle.Id;

            await UniTask.WhenAny(WaitForCollision(particle, cts.Token), WaitForDelay(delay, cts.Token));

            cts.Cancel();
            cts.Dispose();

            await UniTask.Yield();

            abilitiesService.ClearAbilities(particleId);

            if (particle != null)
            {
                particleFactory.DestroyParticle(particle);
            }
        }

        private async UniTask WaitForDelay(float delay, CancellationToken cancellationToken)
        {
            await UniTaskHelper.DelayWithGameplayTimeScale(delay, timeScaleHolder, cancellationToken);
        }

        private async UniTask WaitForCollision(ParticleView particle, CancellationToken cancellationToken)
        {
            Collider collider = particle.GetComponent<Collider>();
            Transform transform = particle.transform;

            while (particle != null && !collisionDetectionService.IsCollisionInBoxDetected(
                transform.position,
                collider.bounds.size,
                transform.rotation,
                particle.gameObject.layer))
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }
        }
    }
}