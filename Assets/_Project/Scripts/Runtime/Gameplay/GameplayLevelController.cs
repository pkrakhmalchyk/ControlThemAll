using System;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public enum LevelState
    {
        InProgress = 0,
        Win = 1,
        Defeat = 2
    }

    public class GameplayLevelController : ILoadingModule, IDisposable
    {
        public Action LevelWon;
        public Action LevelLost;


        private readonly LevelPlayersService levelPlayersService;
        private readonly LevelPlayersWeaponsService levelPlayersWeaponsService;
        private readonly LevelEnemiesService levelEnemiesService;
        private readonly LevelEnemiesWeaponsService levelEnemiesWeaponsService;
        private readonly LevelEnvironmentService levelEnvironmentService;
        private readonly PlayersMovementController playersMovementController;
        private readonly IDataHandler dataHandler;
        private readonly LevelsDataService levelsDataService;
        private readonly TimeScaleHolder timeScaleHolder;
        private readonly CameraService cameraService;
        private readonly StatsService statsService;


        public LevelState State { get; private set; }


        public GameplayLevelController(
            LevelPlayersService levelPlayersService,
            LevelPlayersWeaponsService levelPlayersWeaponsService,
            LevelEnemiesService levelEnemiesService,
            LevelEnemiesWeaponsService levelEnemiesWeaponsService,
            LevelEnvironmentService levelEnvironmentService,
            PlayersMovementController playersMovementController,
            IDataHandler dataHandler,
            LevelsDataService levelsDataService,
            TimeScaleHolder timeScaleHolder,
            CameraService cameraService,
            StatsService statsService)
        {
            this.levelPlayersService = levelPlayersService;
            this.levelPlayersWeaponsService = levelPlayersWeaponsService;
            this.levelEnemiesService = levelEnemiesService;
            this.levelEnemiesWeaponsService = levelEnemiesWeaponsService;
            this.levelEnvironmentService = levelEnvironmentService;
            this.playersMovementController = playersMovementController;
            this.dataHandler = dataHandler;
            this.levelsDataService = levelsDataService;
            this.timeScaleHolder = timeScaleHolder;
            this.cameraService = cameraService;
            this.statsService = statsService;
        }

        public UniTask Load()
        {
            levelPlayersService.SetPlayersActive(true);
            levelEnemiesService.SetEnemiesActive(true);
            timeScaleHolder.SetGameplayTimeScale(0f);
            cameraService.SetStartGameplayCameraActive();

            State = LevelState.InProgress;

            return UniTask.CompletedTask;
        }

        public void StartGameplay()
        {
            cameraService.SetActiveGameplayCameraActive();
            playersMovementController.SetActive(true);
            levelPlayersWeaponsService.SetWeaponsActive(true);
            levelEnemiesWeaponsService.SetWeaponsActive(true);

            timeScaleHolder.SetGameplayTimeScale(1f);
            timeScaleHolder.SetGameplayVisualEffectsTimeScale(1f);

            levelPlayersService.PlayerDied += OnPlayerDied;
            levelEnvironmentService.LevelEndEntered += OnLevelEndEntered;
            statsService.HealthStatChanged += OnHealthStatChanged;
        }

        public void PauseGameplay()
        {
            timeScaleHolder.SetGameplayTimeScale(0f);
            timeScaleHolder.SetGameplayVisualEffectsTimeScale(0f);
        }

        public void SlowDownGameplay(float scale)
        {
            scale = Mathf.Clamp01(scale);

            timeScaleHolder.SetGameplayTimeScale(scale);
            timeScaleHolder.SetGameplayVisualEffectsTimeScale(scale);
        }

        public void ResumeGameplay()
        {
            timeScaleHolder.SetGameplayTimeScale(1f);
            timeScaleHolder.SetGameplayVisualEffectsTimeScale(1f);
        }

        public void Dispose()
        {
            levelPlayersService.PlayerDied -= OnPlayerDied;
            levelEnvironmentService.LevelEndEntered -= OnLevelEndEntered;
            statsService.HealthStatChanged -= OnHealthStatChanged;
        }


        private void OnPlayerDied(PlayerView player)
        {
            levelPlayersService.PlayerDied -= OnPlayerDied;
            levelEnvironmentService.LevelEndEntered -= OnLevelEndEntered;
            statsService.HealthStatChanged -= OnHealthStatChanged;

            cameraService.SetEndGameplayCameraActive();

            State = LevelState.Defeat;
            LevelLost?.Invoke();
        }

        private async void OnLevelEndEntered(IGameplayEntity entity)
        {
            LayerMask layerMask = LayerMask.GetMask(RuntimeConstants.PhysicLayers.Player);

            if (layerMask == (layerMask | (1 << entity.PhysicsLayer)))
            {
                levelPlayersService.PlayerDied -= OnPlayerDied;
                levelEnvironmentService.LevelEndEntered -= OnLevelEndEntered;
                statsService.HealthStatChanged -= OnHealthStatChanged;

                cameraService.SetEndGameplayCameraActive();
                await UpdateGameProgressData();

                State = LevelState.Win;
                LevelWon?.Invoke();
            }
        }

        private void OnHealthStatChanged(int entityId, float previousValue, float newValue)
        {
            if (levelPlayersService.TryGetPlayerById(entityId, out PlayerView player))
            {
                cameraService.Shake(RuntimeConstants.Settings.CameraShakeAfterPlayerHitDuration);
            }
        }

        private async UniTask UpdateGameProgressData()
        {
            int currentLevelIndex = levelsDataService.GetCurrentLevelIndex();
            GameProgressData gameProgressData = await dataHandler.Load<GameProgressData>(RuntimeConstants.Data.GameProgressData);

            if (gameProgressData == null)
            {
                gameProgressData = new GameProgressData
                {
                    CurrentLevel = currentLevelIndex
                };
            }

            if (gameProgressData.CurrentLevel == currentLevelIndex)
            {
                gameProgressData.CurrentLevel++;

                await dataHandler.Save(RuntimeConstants.Data.GameProgressData, gameProgressData);
            }
        }
    }
}
