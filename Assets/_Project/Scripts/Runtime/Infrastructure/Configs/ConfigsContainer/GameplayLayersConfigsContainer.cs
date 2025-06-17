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
    public class GameplayLayersConfigsContainer : ILoadingModule
    {
        private Dictionary<string, DefaultGameplayLayerConfig> defaultGameplayLayersConfigs;
        private Dictionary<string, EnemyGameplayLayerConfig> enemyGameplayLayersConfigs;
        private Dictionary<string, PlayerGameplayLayerConfig> playerGameplayLayersConfigs;


        public async UniTask Load()
        {
            await LoadConfigs();
        }

        public Color GetEntityGameplayLayerMainColor(IGameplayEntity entity)
        {
            string entityPhysicsLayerName = LayerMask.LayerToName(entity.PhysicsLayer);

            if (entityPhysicsLayerName == RuntimeConstants.PhysicLayers.Player)
            {
                return GetPlayerGameplayLayerConfig(entity.GameplayLayer).FillColor;
            }

            if (entityPhysicsLayerName == RuntimeConstants.PhysicLayers.Enemy)
            {
                return GetEnemyGameplayLayerConfig(entity.GameplayLayer).FillColor;
            }

            return GetDefaultGameplayLayerConfig(entity.GameplayLayer).Color;
        }

        public DefaultGameplayLayerConfig GetDefaultGameplayLayerConfig(string gameplayLayer)
        {
            if (!defaultGameplayLayersConfigs.TryGetValue(gameplayLayer, out DefaultGameplayLayerConfig defaultGameplayLayerConfig))
            {
                Logging.Gameplay.LogError($"Default gameplay layer {gameplayLayer} was not found");

                throw new Exception($"Default gameplay layer {gameplayLayer} was not found");
            }

            return defaultGameplayLayerConfig;
        }

        public EnemyGameplayLayerConfig GetEnemyGameplayLayerConfig(string gameplayLayer)
        {
            if (!enemyGameplayLayersConfigs.TryGetValue(gameplayLayer, out EnemyGameplayLayerConfig enemyGameplayLayerConfig))
            {
                Logging.Gameplay.LogError($"Enemy gameplay layer {gameplayLayer} was not found");

                throw new Exception($"Enemy gameplay layer {gameplayLayer} was not found");
            }

            return enemyGameplayLayerConfig;
        }

        public PlayerGameplayLayerConfig GetPlayerGameplayLayerConfig(string gameplayLayer)
        {
            if (!playerGameplayLayersConfigs.TryGetValue(gameplayLayer, out PlayerGameplayLayerConfig playerGameplayLayerConfig))
            {
                Logging.Gameplay.LogError($"Player gameplay layer {gameplayLayer} was not found");

                throw new Exception($"Player gameplay layer {gameplayLayer} was not found");
            }

            return playerGameplayLayerConfig;
        }

        private async UniTask LoadConfigs()
        {
            defaultGameplayLayersConfigs = new Dictionary<string, DefaultGameplayLayerConfig>();
            AsyncOperationHandle<TextAsset> defaultGameplayLayersConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{RuntimeConstants.Configs.AddressablesTextConfigsPath}/{RuntimeConstants.Configs.DefaultGameplayLayersConfigName}");

            enemyGameplayLayersConfigs = new Dictionary<string, EnemyGameplayLayerConfig>();
            AsyncOperationHandle<TextAsset> enemyGameplayLayersConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{RuntimeConstants.Configs.AddressablesTextConfigsPath}/{RuntimeConstants.Configs.EnemyGameplayLayersConfigName}");


            playerGameplayLayersConfigs = new Dictionary<string, PlayerGameplayLayerConfig>();
            AsyncOperationHandle<TextAsset> playerGameplayLayersConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{RuntimeConstants.Configs.AddressablesTextConfigsPath}/{RuntimeConstants.Configs.PlayerGameplayLayersConfigName}");

            TextAsset defaultGameplayLayersConfigsText = await defaultGameplayLayersConfigsHandle;
            JsonConvert.PopulateObject(defaultGameplayLayersConfigsText.text, defaultGameplayLayersConfigs);
            Addressables.Release(defaultGameplayLayersConfigsHandle);

            TextAsset enemyGameplayLayersConfigsText = await enemyGameplayLayersConfigsHandle;
            JsonConvert.PopulateObject(enemyGameplayLayersConfigsText.text, enemyGameplayLayersConfigs);
            Addressables.Release(enemyGameplayLayersConfigsHandle);

            TextAsset playerGameplayLayersConfigsText = await playerGameplayLayersConfigsHandle;
            JsonConvert.PopulateObject(playerGameplayLayersConfigsText.text, playerGameplayLayersConfigs);
            Addressables.Release(playerGameplayLayersConfigsHandle);
        }
    }
}