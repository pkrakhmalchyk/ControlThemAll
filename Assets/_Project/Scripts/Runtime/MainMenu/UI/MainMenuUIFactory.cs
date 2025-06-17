using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using Object = UnityEngine.Object;

namespace ControllThemAll.Runtime.MainMenu.UI
{
    public class MainMenuUIFactory : ILoadingModule, IDisposable
    {
        private readonly IObjectResolver container;

        private MainMenuUIView mainMenuUIPrefab;
        private AsyncOperationHandle<GameObject> mainMenuUIPrefabHandle;
        private LevelSelectionUIView levelSelectionUIPrefab;
        private AsyncOperationHandle<GameObject> levelSelectionUIPrefabHandle;


        public MainMenuUIFactory(IObjectResolver container)
        {
            this.container = container;
        }

        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public MainMenuUIAdapter CreateMainMenuUI()
        {
            MainMenuUIView mainMenuUIView = Object.Instantiate(mainMenuUIPrefab);
            MainMenuUIAdapter mainMenuUIAdapter = new MainMenuUIAdapter(
                mainMenuUIView,
                container.Resolve<UIService>());

            return mainMenuUIAdapter;
        }

        public LevelSelectionUIAdapter CreateLevelSelectionUI()
        {
            LevelSelectionUIView levelSelectionUIView = Object.Instantiate(levelSelectionUIPrefab);
            LevelSelectionUIAdapter levelSelectionUIAdapter = new LevelSelectionUIAdapter(
                levelSelectionUIView,
                container.Resolve<UIService>(),
                container.Resolve<IDataHandler>(),
                container.Resolve<LevelsDataService>());

            return levelSelectionUIAdapter;
        }

        public void Dispose()
        {
            Addressables.Release(mainMenuUIPrefabHandle);
            Addressables.Release(levelSelectionUIPrefabHandle);
        }


        private async UniTask LoadPrefabs()
        {
            mainMenuUIPrefabHandle =
                Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.UI.AddressablesMainMenuUIPath}/{RuntimeConstants.UI.MainMenuWindowPrefabName}");
            levelSelectionUIPrefabHandle =
                Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.UI.AddressablesMainMenuUIPath}/{RuntimeConstants.UI.LevelSelectionWindowPrefabName}");

            (GameObject mainMenuUIPrefabGo, GameObject levelSelectionUIPrefabGo) = await (
                mainMenuUIPrefabHandle.ToUniTask(), levelSelectionUIPrefabHandle.ToUniTask());

            mainMenuUIPrefab = mainMenuUIPrefabGo.GetComponent<MainMenuUIView>();
            levelSelectionUIPrefab = levelSelectionUIPrefabGo.GetComponent<LevelSelectionUIView>();
        }
    }
}
