using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class EnemyFactory : ILoadingModule, IDisposable
    {
        private readonly IObjectResolver container;

        private Dictionary<string, EnemyView> enemiesPrefabs;
        private List<AsyncOperationHandle<GameObject>> enemiesPrefabsHandles;


        public EnemyFactory(IObjectResolver container)
        {
            this.container = container;
        }

        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public EnemyView CreateEnemy(EnemyConfig config)
        {
            if (!enemiesPrefabs.TryGetValue(config.PrefabName, out EnemyView enemyPrefab))
            {
                Logging.Gameplay.LogError($"Enemy prefab with name {config.PrefabName} was not found");

                throw new Exception($"Enemy prefab with name {config.PrefabName} was not found");
            }

            EnemyView enemy = UnityEngine.Object.Instantiate(enemyPrefab);
            enemy.gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.Enemy);
            enemy.gameObject.SetActive(false);

            enemy.Initialize(
                container.Resolve<IdGenerator>().GetNextId(),
                config,
                new ConstantDirectionalMovementInput(Vector3.forward * -1),
                container.Resolve<TimeScaleHolder>());

            return enemy;
        }

        public void Dispose()
        {
            foreach (AsyncOperationHandle<GameObject> enemyPrefabHandle in enemiesPrefabsHandles)
            {
                Addressables.Release(enemyPrefabHandle);
            }
        }


        private async UniTask LoadPrefabs()
        {
            enemiesPrefabs = new Dictionary<string, EnemyView>();
            enemiesPrefabsHandles = new List<AsyncOperationHandle<GameObject>>();

            foreach (string prefabName in RuntimeConstants.Enemies.All)
            {
                enemiesPrefabsHandles.Add(Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.Enemies.EnemiesPath}/{prefabName}"));
            }

            IEnumerable<GameObject> loadedEnemiesGameObjects = await UniTask.WhenAll(enemiesPrefabsHandles.Select(prefabHandle => prefabHandle.ToUniTask()));
            IEnumerable<EnemyView> loadedEnemiesPrefabs = loadedEnemiesGameObjects.Select(gameObject => gameObject.GetComponent<EnemyView>());

            foreach (EnemyView enemyPrefab in loadedEnemiesPrefabs)
            {
                enemiesPrefabs.Add(enemyPrefab.name, enemyPrefab);
            }
        }
    }
}