using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Gameplay.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace ControllThemAll.Runtime.Gameplay
{
    public class GameplayEntryPoint : IAsyncStartable
    {
        private readonly ModulesLoader modulesLoader;
        private readonly GameplayLevelController gameplayLevelController;
        private readonly PlayerFactory playerFactory;
        private readonly WeaponFactory weaponFactory;
        private readonly ParticleFactory particleFactory;
        private readonly EnemyFactory enemyFactory;
        private readonly GameplayUIFactory gameplayUIFactory;
        private readonly GameplayUIInstaller gameplayUIInstaller;
        private readonly LevelEnvironmentFactory levelEnvironmentFactory;
        private readonly LevelsDataService levelsDataService;
        private readonly EnemiesConfigsContainer enemiesConfigsContainer;
        private readonly EnvironmentConfigsContainer environmentConfigsContainer;
        private readonly GameplayLayersConfigsContainer gameplayLayersConfigsContainer;
        private readonly PlayersConfigsContainer playersConfigsContainer;
        private readonly ParticlesConfigsContainer particlesConfigsContainer;
        private readonly UIService uiService;
        private readonly AimRenderingService aimRenderingService;
        private readonly VfxFactory vfxFactory;
        private readonly PlayersVisualController playersVisualController;
        private readonly LevelPlayersService levelPlayersService;
        private readonly LevelPlayersWeaponsService levelPlayersWeaponsService;
        private readonly LevelEnemiesService levelEnemiesService;
        private readonly LevelEnemiesWeaponsService levelEnemiesWeaponsService;
        private readonly EnemiesVisualController enemiesVisualController;
        private readonly GameplayLayersService gameplayLayersService;
        private readonly LevelEnvironmentService levelEnvironmentService;
        private readonly CameraService cameraService;
        private readonly VfxService vfxService;
        private readonly SwipeHorizontalMovementInput swipeHorizontalMovementInput;
        private readonly DragAttackInput dragAttackInput;

        public GameplayEntryPoint(
            ModulesLoader modulesLoader,
            GameplayLevelController gameplayLevelController,
            PlayerFactory playerFactory,
            WeaponFactory weaponFactory,
            ParticleFactory particleFactory,
            EnemyFactory enemyFactory,
            GameplayUIFactory gameplayUIFactory,
            GameplayUIInstaller gameplayUIInstaller,
            LevelEnvironmentFactory levelEnvironmentFactory,
            LevelsDataService levelsDataService,
            EnemiesConfigsContainer enemiesConfigsContainer,
            EnvironmentConfigsContainer environmentConfigsContainer,
            GameplayLayersConfigsContainer gameplayLayersConfigsContainer,
            PlayersConfigsContainer playersConfigsContainer,
            ParticlesConfigsContainer particlesConfigsContainer,
            UIService uiService,
            AimRenderingService aimRenderingService,
            VfxFactory vfxFactory,
            PlayersVisualController playersVisualController,
            LevelPlayersService levelPlayersService,
            LevelPlayersWeaponsService levelPlayersWeaponsService,
            LevelEnemiesService levelEnemiesService,
            LevelEnemiesWeaponsService levelEnemiesWeaponsService,
            EnemiesVisualController enemiesVisualController,
            GameplayLayersService gameplayLayersService,
            LevelEnvironmentService levelEnvironmentService,
            CameraService cameraService,
            VfxService vfxService,
            DragAttackInput dragAttackInput,
            SwipeHorizontalMovementInput swipeHorizontalMovementInput)
        {
            this.modulesLoader = modulesLoader;
            this.gameplayLevelController = gameplayLevelController;
            this.playerFactory = playerFactory;
            this.weaponFactory = weaponFactory;
            this.particleFactory = particleFactory;
            this.enemyFactory = enemyFactory;
            this.gameplayUIFactory = gameplayUIFactory;
            this.gameplayUIInstaller = gameplayUIInstaller;
            this.levelEnvironmentFactory = levelEnvironmentFactory;
            this.levelsDataService = levelsDataService;
            this.enemiesConfigsContainer = enemiesConfigsContainer;
            this.environmentConfigsContainer = environmentConfigsContainer;
            this.gameplayLayersConfigsContainer = gameplayLayersConfigsContainer;
            this.playersConfigsContainer = playersConfigsContainer;
            this.particlesConfigsContainer = particlesConfigsContainer;
            this.uiService = uiService;
            this.aimRenderingService = aimRenderingService;
            this.vfxFactory = vfxFactory;
            this.playersVisualController = playersVisualController;
            this.levelPlayersService = levelPlayersService;
            this.levelPlayersWeaponsService = levelPlayersWeaponsService;
            this.levelEnemiesService = levelEnemiesService;
            this.levelEnemiesWeaponsService = levelEnemiesWeaponsService;
            this.enemiesVisualController = enemiesVisualController;
            this.gameplayLayersService = gameplayLayersService;
            this.levelEnvironmentService = levelEnvironmentService;
            this.cameraService = cameraService;
            this.vfxService = vfxService;
            this.dragAttackInput = dragAttackInput;
            this.swipeHorizontalMovementInput = swipeHorizontalMovementInput;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            uiService.SetLoadingWindowVisible(true);

            await LoadConfigs();
            await LoadFactories();
            await modulesLoader.Load(dragAttackInput);
            await modulesLoader.Load(swipeHorizontalMovementInput);
            await modulesLoader.Load(aimRenderingService);
            await modulesLoader.Load(levelPlayersService);
            await modulesLoader.Load(levelPlayersWeaponsService);
            await modulesLoader.Load(levelEnemiesService);
            await modulesLoader.Load(levelEnemiesWeaponsService);
            await modulesLoader.Load(gameplayLayersService);
            await modulesLoader.Load(playersVisualController);
            await modulesLoader.Load(enemiesVisualController);
            await modulesLoader.Load(levelEnvironmentService);
            await modulesLoader.Load(vfxService);
            await modulesLoader.Load(cameraService, levelPlayersService.Players.Select(player => player.transform));
            await modulesLoader.Load(gameplayUIInstaller);
            await modulesLoader.Load(gameplayLevelController);

            uiService.SetLoadingWindowVisible(false);

            gameplayUIInstaller.ShowGameplayLevelInitialUI();
        }


        private async UniTask LoadConfigs()
        {
            string levelConfigsPath = levelsDataService.GetCurrentLevelConfig()?.LevelConfigsPath ?? RuntimeConstants.Configs.AddressablesDefaultLevelConfigsPath;
            List<UniTask> tasks = new List<UniTask>
            {
                modulesLoader.Load(enemiesConfigsContainer, levelConfigsPath),
                modulesLoader.Load(environmentConfigsContainer, levelConfigsPath),
                modulesLoader.Load(gameplayLayersConfigsContainer),
                modulesLoader.Load(playersConfigsContainer, levelConfigsPath),
                modulesLoader.Load(particlesConfigsContainer)
            };

            await tasks;
        }

        private async UniTask LoadFactories()
        {
            List<UniTask> tasks = new List<UniTask>
            {
                modulesLoader.Load(playerFactory),
                modulesLoader.Load(weaponFactory),
                modulesLoader.Load(particleFactory),
                modulesLoader.Load(enemyFactory),
                modulesLoader.Load(levelEnvironmentFactory),
                modulesLoader.Load(vfxFactory),
                modulesLoader.Load(gameplayUIFactory)
            };

            await tasks;
        }
    }
}

