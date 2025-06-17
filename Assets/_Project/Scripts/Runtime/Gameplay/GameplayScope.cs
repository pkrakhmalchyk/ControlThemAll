using ControllThemAll.Runtime.Gameplay.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ControllThemAll.Runtime.Gameplay
{
    public class GameplayScope : LifetimeScope
    {
        [SerializeField] private CameraService cameraService;


        protected override void Configure(IContainerBuilder builder)
        {
            RegisterFactories(builder);
            RegisterAttackModules(builder);
            RegisterLevelModules(builder);
            RegisterUtils(builder);
            RegisterUI(builder);
            RegisterConfigsContainers(builder);
            RegisterInputs(builder);

            builder.Register<InventorySystem<PlayerWeaponConfig>>(Lifetime.Singleton);
            builder.Register<AbilitiesService>(Lifetime.Singleton);
            builder.Register<StatsService>(Lifetime.Singleton);
            builder.Register<VfxService>(Lifetime.Singleton);
            builder.Register<TimeScaleHolder>(Lifetime.Singleton);
            builder.RegisterComponent(cameraService);

            builder.RegisterEntryPoint<GameplayEntryPoint>();
        }


        private void RegisterUI(IContainerBuilder builder)
        {
            builder.Register<GameplayUIFactory>(Lifetime.Singleton);
            builder.Register<GameplayUIInstaller>(Lifetime.Singleton);
        }

        private void RegisterFactories(IContainerBuilder builder)
        {
            builder.Register<PlayerFactory>(Lifetime.Singleton);
            builder.Register<ParticleFactory>(Lifetime.Singleton);
            builder.Register<AbilityFactory>(Lifetime.Singleton);
            builder.Register<WeaponFactory>(Lifetime.Singleton);
            builder.Register<EnemyFactory>(Lifetime.Singleton);
            builder.Register<LevelEnvironmentFactory>(Lifetime.Singleton);
            builder.Register<VfxFactory>(Lifetime.Singleton);
        }

        private void RegisterAttackModules(IContainerBuilder builder)
        {
            builder.Register<ShootingService>(Lifetime.Singleton);
            builder.Register<AimRenderingService>(Lifetime.Singleton);
            builder.Register<AttackTargetsProvider>(Lifetime.Singleton);
            builder.Register<AttackService>(Lifetime.Singleton);
        }

        private void RegisterLevelModules(IContainerBuilder builder)
        {
            builder.Register<GameplayLevelController>(Lifetime.Singleton);
            builder.Register<LevelPlayersService>(Lifetime.Singleton);
            builder.Register<LevelEnemiesService>(Lifetime.Singleton);
            builder.Register<LevelEnemiesWeaponsService>(Lifetime.Singleton);
            builder.Register<EnemiesVisualController>(Lifetime.Singleton);
            builder.Register<PlayersVisualController>(Lifetime.Singleton);
            builder.Register<PlayersMovementController>(Lifetime.Singleton);
            builder.Register<GameplayLayersService>(Lifetime.Singleton);
            builder.Register<LevelPlayersWeaponsService>(Lifetime.Singleton);
            builder.Register<LevelEnvironmentService>(Lifetime.Singleton);
        }

        private void RegisterUtils(IContainerBuilder builder)
        {
            builder.Register<IdGenerator>(Lifetime.Scoped);
            builder.Register<CollisionDetectionService>(Lifetime.Singleton);
        }

        private void RegisterConfigsContainers(IContainerBuilder builder)
        {
            builder.Register<EnemiesConfigsContainer>(Lifetime.Singleton);
            builder.Register<EnvironmentConfigsContainer>(Lifetime.Singleton);
            builder.Register<GameplayLayersConfigsContainer>(Lifetime.Singleton);
            builder.Register<PlayersConfigsContainer>(Lifetime.Singleton);
            builder.Register<ParticlesConfigsContainer>(Lifetime.Singleton);
        }

        private void RegisterInputs(IContainerBuilder builder)
        {
            builder.Register<ITouchInput, MouseTouchInput>(Lifetime.Singleton);
            builder.Register(container =>
                    new DragAttackInput(
                        container.Resolve<CameraService>(),
                        container.Resolve<ITouchInput>(),
                        LayerMask.GetMask(RuntimeConstants.PhysicLayers.Player)), Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.Register<SwipeHorizontalMovementInput>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
        }
    }
}
