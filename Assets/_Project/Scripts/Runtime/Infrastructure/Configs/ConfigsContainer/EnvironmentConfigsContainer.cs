using System.Collections.Generic;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ControllThemAll.Runtime.Infrastructure
{
    public class EnvironmentConfigsContainer : ILoadingModule<string>
    {
        private List<LevelBrickConfig> levelBricksConfigs;
        private List<LevelEnvironmentPartConfig> levelEnvironmentPartsConfigs;


        public async UniTask Load(string levelConfigsPath)
        {
            await LoadConfigs(levelConfigsPath);
        }

        public IReadOnlyList<LevelBrickConfig> GetLevelBricksConfigs()
        {
            return levelBricksConfigs;
        }

        public IReadOnlyList<LevelEnvironmentPartConfig> GetLevelEnvironmentPartsConfigs()
        {
            return levelEnvironmentPartsConfigs;
        }


        private async UniTask LoadConfigs(string levelConfigsPath)
        {
            levelBricksConfigs = new List<LevelBrickConfig>();
            AsyncOperationHandle<TextAsset> levelBricksConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{levelConfigsPath}/{RuntimeConstants.Configs.LevelBricksConfigName}");

            levelEnvironmentPartsConfigs = new List<LevelEnvironmentPartConfig>();
            AsyncOperationHandle<TextAsset> levelEnvironmentPartsConfigsHandle = Addressables.LoadAssetAsync<TextAsset>(
                $"{levelConfigsPath}/{RuntimeConstants.Configs.LevelEnvironmentPartsConfigName}");

            TextAsset levelBricksConfigsText = await levelBricksConfigsHandle;
            JsonConvert.PopulateObject(levelBricksConfigsText.text, levelBricksConfigs);
            Addressables.Release(levelBricksConfigsHandle);

            TextAsset levelEnvironmentPartsConfigsText = await levelEnvironmentPartsConfigsHandle;
            JsonConvert.PopulateObject(levelEnvironmentPartsConfigsText.text, levelEnvironmentPartsConfigs);
            Addressables.Release(levelEnvironmentPartsConfigsHandle);
        }
    }
}