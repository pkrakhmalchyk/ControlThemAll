using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Gameplay;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ControllThemAll.Runtime.Infrastructure
{
    public class EnemiesConfigsContainer : ILoadingModule<string>, IDisposable
    {
        private Dictionary<string, EnemyConfig> enemiesConfigs;
        private List<LevelEnemyConfig> levelEnemiesConfigs;
        private Dictionary<string, EnemyWeaponConfig> enemiesWeaponsConfigs;
        private Dictionary<string, AbilityConfig<EnemyWeaponContext>> enemiesWeaponsAbilitiesConfigs;

        private AsyncOperationHandle<IList<EnemyConfig>> enemiesConfigsHandle;
        private AsyncOperationHandle<IList<EnemyWeaponConfig>> enemiesWeaponsConfigsHandle;


        public async UniTask Load(string levelConfigsPath)
        {
            await LoadConfigs(levelConfigsPath);
        }

        public EnemyConfig GetEnemyConfig(string id)
        {
            if (!enemiesConfigs.TryGetValue(id, out EnemyConfig enemyConfig))
            {
                Logging.Gameplay.LogError($"Enemy config '{id}' was not found");

                throw new Exception($"Enemy config '{id}' was not found");
            }

            return enemyConfig;
        }

        public IReadOnlyList<LevelEnemyConfig> GetLevelEnemiesConfigs()
        {
            return levelEnemiesConfigs;
        }

        public EnemyWeaponConfig GetEnemyWeaponConfig(string id)
        {
            if (!enemiesWeaponsConfigs.TryGetValue(id, out EnemyWeaponConfig enemyWeaponConfig))
            {
                Logging.Gameplay.LogError($"Enemy weapon config '{id}' was not found");

                throw new Exception($"Enemy weapon config '{id}' was not found");
            }

            return enemyWeaponConfig;
        }

        public AbilityConfig<EnemyWeaponContext> GetEnemyWeaponAbilityConfig(string id)
        {
            if (!enemiesWeaponsAbilitiesConfigs.TryGetValue(id, out AbilityConfig<EnemyWeaponContext> enemyWeaponAbilityConfig))
            {
                Logging.Gameplay.LogError($"Enemy weapon ability config '{id}' was not found");

                throw new Exception($"Enemy weapon ability config '{id}' was not found");
            }

            return enemyWeaponAbilityConfig;
        }

        public void Dispose()
        {
            Addressables.Release(enemiesConfigsHandle);
            Addressables.Release(enemiesWeaponsConfigsHandle);
        }


        private async UniTask LoadConfigs(string levelConfigsPath)
        {
            enemiesConfigs = new Dictionary<string, EnemyConfig>();
            enemiesConfigsHandle = Addressables.LoadAssetsAsync<EnemyConfig>(new []
            {
                $"{RuntimeConstants.Configs.EnemiesConfigsLabel}"
            }, null, Addressables.MergeMode.Union);

            levelEnemiesConfigs = new List<LevelEnemyConfig>();
            AsyncOperationHandle<TextAsset> levelEnemiesConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{levelConfigsPath}/{RuntimeConstants.Configs.LevelEnemiesConfigName}");

            enemiesWeaponsConfigs = new Dictionary<string, EnemyWeaponConfig>();
            enemiesWeaponsConfigsHandle = Addressables.LoadAssetsAsync<EnemyWeaponConfig>(new []
            {
                $"{RuntimeConstants.Configs.EnemiesWeaponsConfigsLabel}"
            }, null, Addressables.MergeMode.Union);

            enemiesWeaponsAbilitiesConfigs = new Dictionary<string, AbilityConfig<EnemyWeaponContext>>();
            AsyncOperationHandle<IList<TextAsset>> enemiesWeaponsAbilitiesConfigsHandle = Addressables.LoadAssetsAsync<TextAsset>(new []
            {
                $"{RuntimeConstants.Configs.EnemiesWeaponsAbilitiesConfigsLabel}",
            }, null, Addressables.MergeMode.Union );

            IList<EnemyConfig> loadedEnemiesConfigs = await enemiesConfigsHandle;

            foreach (EnemyConfig enemyConfig in loadedEnemiesConfigs)
            {
                enemiesConfigs.Add(enemyConfig.name, enemyConfig);
            }

            TextAsset levelEnemiesConfigsText = await levelEnemiesConfigsHandle;
            JsonConvert.PopulateObject(levelEnemiesConfigsText.text, levelEnemiesConfigs);
            Addressables.Release(levelEnemiesConfigsHandle);

            IList<EnemyWeaponConfig> loadedEnemiesWeaponsConfigs = await enemiesWeaponsConfigsHandle;

            foreach (EnemyWeaponConfig enemyWeaponConfig in loadedEnemiesWeaponsConfigs)
            {
                enemiesWeaponsConfigs.Add(enemyWeaponConfig.name, enemyWeaponConfig);
            }

            IList<TextAsset> enemiesWeaponsAbilitiesConfigsTexts = await enemiesWeaponsAbilitiesConfigsHandle;

            foreach (TextAsset enemiesWeaponsAbilityConfigText in enemiesWeaponsAbilitiesConfigsTexts)
            {
                AbilityConfig<EnemyWeaponContext> abilityConfig = JsonConvert.DeserializeObject<AbilityConfig<EnemyWeaponContext>>(enemiesWeaponsAbilityConfigText.text, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                enemiesWeaponsAbilitiesConfigs.Add(enemiesWeaponsAbilityConfigText.name, abilityConfig);
            }

            Addressables.Release(enemiesWeaponsAbilitiesConfigsHandle);
        }
    }
}