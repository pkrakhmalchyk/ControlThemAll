using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.MainMenu;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ControllThemAll.Runtime.Shared
{
    public class LevelsDataService : ILoadingModule
    {
        private List<LevelConfig> levelsConfigs;
        private LevelConfig currentLevelConfig;
        private int currentLevelIndex = -1;


        public async UniTask Load()
        {
            await LoadConfigs();
        }

        public LevelConfig GetCurrentLevelConfig()
        {
            return currentLevelConfig;
        }

        public int GetCurrentLevelIndex()
        {
            return currentLevelIndex;
        }

        public IReadOnlyList<LevelConfig> GetAllLevelConfigs()
        {
            return levelsConfigs;
        }

        public void SetCurrentLevelByName(string name)
        {
            for (int i = 0; i < levelsConfigs.Count; i++)
            {
                if (levelsConfigs[i].LevelName == name)
                {
                    currentLevelConfig = levelsConfigs[i];
                    currentLevelIndex = i;

                    return;
                }
            }

            currentLevelConfig = null;
            currentLevelIndex = -1;
        }

        public void SetCurrentLevelByIndex(int index)
        {
            if (index < 0 || index >= levelsConfigs.Count)
            {
                currentLevelConfig = null;
                currentLevelIndex = -1;
            }

            currentLevelConfig = levelsConfigs[index];
            currentLevelIndex = index;
        }


        private async UniTask LoadConfigs()
        {
            levelsConfigs = new List<LevelConfig>();
            AsyncOperationHandle<TextAsset> levelsConfigsHandle =
                Addressables.LoadAssetAsync<TextAsset>($"{RuntimeConstants.Configs.AddressablesTextConfigsPath}/{RuntimeConstants.Configs.LevelsConfigsName}");
            TextAsset levelsConfigsText = await levelsConfigsHandle;

            if (levelsConfigsText == null)
            {
                Logging.Gameplay.LogError($"Levels configs were not found");

                throw new Exception($"Levels configs were not found");
            }

            JsonConvert.PopulateObject(levelsConfigsText.text, levelsConfigs);
            Addressables.Release(levelsConfigsHandle);
        }
    }
}