using System;
using System.Collections.Generic;
using System.Threading;
using ControllThemAll.Runtime.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class LevelEnvironmentService : ILoadingModule, IDisposable
    {
        public Action<IGameplayEntity> LevelEndEntered;


        private readonly LevelEnvironmentFactory levelEnvironmentFactory;
        private readonly EnvironmentConfigsContainer environmentConfigsContainer;
        private readonly GameplayLayersConfigsContainer gameplayLayersConfigsContainer;
        private readonly LevelPlayersService levelPlayersService;

        private List<BrickView> bricks;
        private LevelEndTriggerView levelEndTrigger;
        private GameObject enemyExplosionTrigger;
        private GameObject levelGroundParentGo;
        private CancellationTokenSource enemyExplosionTriggerCts;
        private float furthestBrickPosition;


        public LevelEnvironmentService(
            LevelEnvironmentFactory levelEnvironmentFactory,
            EnvironmentConfigsContainer environmentConfigsContainer,
            GameplayLayersConfigsContainer gameplayLayersConfigsContainer,
            LevelPlayersService levelPlayersService)
        {
            this.levelEnvironmentFactory = levelEnvironmentFactory;
            this.environmentConfigsContainer = environmentConfigsContainer;
            this.gameplayLayersConfigsContainer = gameplayLayersConfigsContainer;
            this.levelPlayersService = levelPlayersService;
        }

        public UniTask Load()
        {
            levelGroundParentGo = new GameObject("LevelEnvironment");

            InstantiateBricks();
            InstantiateEndLevelTrigger();
            InstantiateEnvironmentParts();
            InstantiateEnemyExplosionTrigger();

            levelEndTrigger.TriggerEntered += OnLevelEndEntered;

            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            levelEndTrigger.TriggerEntered -= OnLevelEndEntered;

            enemyExplosionTriggerCts?.Cancel();
            enemyExplosionTriggerCts?.Dispose();

            enemyExplosionTriggerCts = null;
        }


        private void InstantiateBricks()
        {
            bricks = new List<BrickView>();

            foreach (LevelBrickConfig levelBrickConfig in environmentConfigsContainer.GetLevelBricksConfigs())
            {
                BrickView brick = levelEnvironmentFactory.CreateBrick();

                brick.transform.SetParent(levelGroundParentGo.transform);
                brick.transform.position = levelBrickConfig.Position;
                brick.transform.localScale = levelBrickConfig.Scale;

                DefaultGameplayLayerConfig defaultGameplayLayerConfig =
                    gameplayLayersConfigsContainer.GetDefaultGameplayLayerConfig(levelBrickConfig.GameplayLayer);

                brick.SetGameplayLayer(levelBrickConfig.GameplayLayer);
                brick.SetColor(defaultGameplayLayerConfig.Color);
                brick.gameObject.SetActive(true);

                bricks.Add(brick);

                float maxBounds = brick.GetBounds().max.z;

                if (maxBounds > furthestBrickPosition)
                {
                    furthestBrickPosition = maxBounds;
                }
            }
        }

        private void InstantiateEnvironmentParts()
        {
            foreach (LevelEnvironmentPartConfig levelEnvironmentPartConfig in environmentConfigsContainer.GetLevelEnvironmentPartsConfigs())
            {
                GameObject environmentPart = levelEnvironmentFactory.CreateEnvironmentPart(levelEnvironmentPartConfig.PrefabName);

                environmentPart.transform.SetParent(levelGroundParentGo.transform);
                environmentPart.transform.position = levelEnvironmentPartConfig.Position;
                environmentPart.transform.localScale = levelEnvironmentPartConfig.Scale;
                environmentPart.gameObject.SetActive(true);
            }
        }

        private void InstantiateEndLevelTrigger()
        {
            float levelEndWidth = GetBricksWidthAtZPosition(furthestBrickPosition);
            levelEndTrigger = levelEnvironmentFactory.CreateLevelEndTrigger();

            levelEndTrigger.transform.position = new Vector3(0, 0, furthestBrickPosition);
            levelEndTrigger.transform.SetParent(levelGroundParentGo.transform);
            levelEndTrigger.SetWidth(levelEndWidth);
            levelEndTrigger.gameObject.SetActive(true);
        }

        private void InstantiateEnemyExplosionTrigger()
        {
            enemyExplosionTrigger = levelEnvironmentFactory.CreateEnemyExplosionTrigger();

            enemyExplosionTrigger.transform.position = Vector3.zero;
            enemyExplosionTrigger.transform.SetParent(levelGroundParentGo.transform);
            enemyExplosionTrigger.gameObject.SetActive(true);

            enemyExplosionTriggerCts = new CancellationTokenSource();

            SyncEnemyExplosionTriggerPositionWithPlayers(enemyExplosionTriggerCts.Token).Forget();
        }

        private float GetBricksWidthAtZPosition(float positionZ)
        {
            float width = 0;

            foreach (BrickView brick in bricks)
            {
                Bounds bounds = brick.GetBounds();

                if (bounds.min.z <= positionZ && bounds.max.z >= positionZ)
                {
                    width += bounds.extents.x * 2;
                }
            }

            return width;
        }

        private void OnLevelEndEntered(Collider collider)
        {
            if (collider.TryGetComponent(out IGameplayEntity entity))
            {
                LevelEndEntered?.Invoke(entity);
            }
        }

        private async UniTask SyncEnemyExplosionTriggerPositionWithPlayers(CancellationToken cancellationToken)
        {
            Transform enemyExplosionTriggerTransform = enemyExplosionTrigger.transform;

            while (!cancellationToken.IsCancellationRequested)
            {
                float closestPositionZ = furthestBrickPosition;

                foreach (PlayerView player in levelPlayersService.Players)
                {
                    if (player.transform.position.z < closestPositionZ)
                    {
                        closestPositionZ = player.transform.position.z;
                    }
                }

                enemyExplosionTriggerTransform.position = new Vector3(0, 0, closestPositionZ);

                await UniTask.Yield();
            }
        }
    }
}