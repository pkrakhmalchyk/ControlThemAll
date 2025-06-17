using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using Object = UnityEngine.Object;

namespace ControllThemAll.Runtime.Gameplay
{
    public class LevelEnvironmentFactory : ILoadingModule, IDisposable
    {
        private readonly IObjectResolver container;

        private BrickView defaultBrickPrefab;
        private AsyncOperationHandle<GameObject> defaultBrickPrefabHandle;
        private LevelEndTriggerView levelEndTriggerPrefab;
        private AsyncOperationHandle<GameObject> levelEndTriggerPrefabHandle;
        private GameObject enemyExplosionTriggerPrefab;
        private AsyncOperationHandle<GameObject> enemyExplosionTriggerPrefabHandle;
        private Dictionary<string, GameObject> environmentParts;
        private AsyncOperationHandle<IList<GameObject>> environmentPartsHandle;


        public LevelEnvironmentFactory(IObjectResolver container)
        {
            this.container = container;
        }


        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public BrickView CreateBrick()
        {
            BrickView brick = Object.Instantiate(defaultBrickPrefab);
            brick.Initialize();
            brick.gameObject.name = defaultBrickPrefab.name;
            brick.gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.Brick);
            brick.gameObject.SetActive(false);

            return brick;
        }

        public LevelEndTriggerView CreateLevelEndTrigger()
        {
            LevelEndTriggerView levelEndTrigger = Object.Instantiate(levelEndTriggerPrefab);
            levelEndTrigger.Initialize(
                container.Resolve<CollisionDetectionService>());
            levelEndTrigger.gameObject.name = levelEndTriggerPrefab.name;
            levelEndTrigger.gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnvironmentPart);
            levelEndTrigger.gameObject.SetActive(false);

            return levelEndTrigger;
        }

        public GameObject CreateEnemyExplosionTrigger()
        {
            GameObject enemyExplosionTrigger = Object.Instantiate(enemyExplosionTriggerPrefab);
            enemyExplosionTrigger.gameObject.name = enemyExplosionTriggerPrefab.name;
            enemyExplosionTrigger.gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnemyExplosionTrigger);
            enemyExplosionTrigger.gameObject.SetActive(false);

            return enemyExplosionTrigger;
        }


        public GameObject CreateEnvironmentPart(string prefabName)
        {
            if (!environmentParts.TryGetValue(prefabName, out GameObject prefab))
            {
                Logging.Gameplay.LogError($"Environment part with name '{prefabName}' was not found");

                throw new Exception($"Environment part with name '{prefabName}' was not found");
            }

            GameObject environmentPart = Object.Instantiate(prefab);
            environmentPart.gameObject.name = prefab.name;
            environmentPart.gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnvironmentPart);
            environmentPart.gameObject.SetActive(false);

            return environmentPart;
        }

        public void Dispose()
        {
            Addressables.Release(defaultBrickPrefabHandle);
            Addressables.Release(levelEndTriggerPrefabHandle);
            Addressables.Release(environmentPartsHandle);
            Addressables.Release(enemyExplosionTriggerPrefabHandle);
        }


        private async UniTask LoadPrefabs()
        {
            defaultBrickPrefabHandle = Addressables.LoadAssetAsync<GameObject>(
                $"{RuntimeConstants.Environment.EnvironmentPath}/{RuntimeConstants.Environment.DefaultBrick}");
            levelEndTriggerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(
                $"{RuntimeConstants.Environment.EnvironmentPath}/{RuntimeConstants.Environment.LevelEndTrigger}");
            enemyExplosionTriggerPrefabHandle = Addressables.LoadAssetAsync<GameObject>(
                $"{RuntimeConstants.Environment.EnvironmentPath}/{RuntimeConstants.Environment.EnemyExplosionTrigger}");
            environmentParts = new Dictionary<string, GameObject>();
            environmentPartsHandle = Addressables.LoadAssetsAsync<GameObject>(new []
            {
                $"{RuntimeConstants.Environment.EnvironmentPartsLabel}"
            }, null, Addressables.MergeMode.Union);

            defaultBrickPrefab = (await defaultBrickPrefabHandle).GetComponent<BrickView>();
            levelEndTriggerPrefab = (await levelEndTriggerPrefabHandle).GetComponent<LevelEndTriggerView>();
            enemyExplosionTriggerPrefab = await enemyExplosionTriggerPrefabHandle;
            IList<GameObject> loadedEnvironmentParts = await environmentPartsHandle;

            foreach (GameObject environmentPart in loadedEnvironmentParts)
            {
                environmentParts.Add(environmentPart.name, environmentPart);
            }
        }
    }
}