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
    public class PlayersConfigsContainer : ILoadingModule<string>, IDisposable
    {
        private PlayerConfig playerConfig;
        private List<LevelPlayerConfig> levelPlayersConfigs;
        private Dictionary<string, PlayerWeaponConfig> playersWeaponsConfigs;
        private List<LevelPlayerWeaponConfig> levelPlayersWeaponsConfigs;
        private Dictionary<string, AbilityConfig<PlayerWeaponContext>> playersWeaponsAbilitiesConfigs;

        private AsyncOperationHandle<IList<PlayerWeaponConfig>> playersWeaponsConfigsHandle;

        public async UniTask Load(string levelConfigsPath)
        {
            await LoadConfigs(levelConfigsPath);
        }

        public PlayerConfig GetPlayerConfig()
        {
            return playerConfig;
        }

        public IReadOnlyList<LevelPlayerConfig> GetLevelPlayersConfigs()
        {
            return levelPlayersConfigs;
        }

        public PlayerWeaponConfig GetPlayerWeaponConfig(string id)
        {
            if (!playersWeaponsConfigs.TryGetValue(id, out PlayerWeaponConfig playerWeaponConfig))
            {
                Logging.Gameplay.LogError($"Player weapon config '{id}' was not found");

                throw new Exception($"Player weapon config '{id}' was not found");
            }

            return playerWeaponConfig;
        }

        public AbilityConfig<PlayerWeaponContext> GetPlayerWeaponAbilityConfig(string id)
        {
            if (!playersWeaponsAbilitiesConfigs.TryGetValue(id, out AbilityConfig<PlayerWeaponContext> playerWeaponAbilityConfig))
            {
                Logging.Gameplay.LogError($"Player weapon ability config '{id}' was not found");

                throw new Exception($"Player weapon ability config '{id}' was not found");
            }

            return playerWeaponAbilityConfig;
        }


        public IReadOnlyList<LevelPlayerWeaponConfig> GetLevelPlayerWeaponsConfigs()
        {
            return levelPlayersWeaponsConfigs;
        }

        public void Dispose()
        {
            Addressables.Release(playersWeaponsConfigsHandle);
        }


        private async UniTask LoadConfigs(string levelConfigsPath)
        {
            playerConfig = new PlayerConfig();
            AsyncOperationHandle<TextAsset> playerConfigHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{RuntimeConstants.Configs.AddressablesTextConfigsPath}/{RuntimeConstants.Configs.PlayerConfigName}");

            levelPlayersConfigs = new List<LevelPlayerConfig>();
            AsyncOperationHandle<TextAsset> levelPlayersConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{levelConfigsPath}/{RuntimeConstants.Configs.LevelPlayersConfigName}");


            playersWeaponsConfigs = new Dictionary<string, PlayerWeaponConfig>();
            playersWeaponsConfigsHandle = Addressables.LoadAssetsAsync<PlayerWeaponConfig>(new []
            {
                $"{RuntimeConstants.Configs.PlayersWeaponsConfigsLabel}",
            }, null, Addressables.MergeMode.Union );

            levelPlayersWeaponsConfigs = new List<LevelPlayerWeaponConfig>();
            AsyncOperationHandle<TextAsset> levelPlayersWeaponsConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{levelConfigsPath}/{RuntimeConstants.Configs.LevelPlayersWeaponsConfigName}");

            playersWeaponsAbilitiesConfigs = new Dictionary<string, AbilityConfig<PlayerWeaponContext>>();
            AsyncOperationHandle<IList<TextAsset>> playersWeaponsAbilitiesConfigsHandle = Addressables.LoadAssetsAsync<TextAsset>(new []
            {
                $"{RuntimeConstants.Configs.PlayersWeaponsAbilitiesConfigsLabel}",
            }, null, Addressables.MergeMode.Union );


            TextAsset playerConfigText = await playerConfigHandle;
            JsonConvert.PopulateObject(playerConfigText.text, playerConfig);
            Addressables.Release(playerConfigHandle);

            TextAsset levelPlayersConfigsText = await levelPlayersConfigsHandle;
            JsonConvert.PopulateObject(levelPlayersConfigsText.text, levelPlayersConfigs);
            Addressables.Release(levelPlayersConfigsHandle);

            IList<PlayerWeaponConfig> loadedPlayerWeaponsConfigs = await playersWeaponsConfigsHandle;
            foreach (PlayerWeaponConfig playerWeaponConfig in loadedPlayerWeaponsConfigs)
            {
                playersWeaponsConfigs.Add(playerWeaponConfig.name, playerWeaponConfig);
            }

            TextAsset levelPlayersWeaponsConfigsText = await levelPlayersWeaponsConfigsHandle;

            JsonConvert.PopulateObject(levelPlayersWeaponsConfigsText.text, levelPlayersWeaponsConfigs);
            Addressables.Release(levelPlayersWeaponsConfigsHandle);

            IList<TextAsset> playersWeaponsAbilitiesConfigsTexts = await playersWeaponsAbilitiesConfigsHandle;

            foreach (TextAsset playersWeaponsAbilityConfigText in playersWeaponsAbilitiesConfigsTexts)
            {
                AbilityConfig<PlayerWeaponContext> abilityConfig = JsonConvert.DeserializeObject<AbilityConfig<PlayerWeaponContext>>(playersWeaponsAbilityConfigText.text, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                playersWeaponsAbilitiesConfigs.Add(playersWeaponsAbilityConfigText.name, abilityConfig);
            }

            Addressables.Release(playersWeaponsAbilitiesConfigsHandle);
        }
    }
}