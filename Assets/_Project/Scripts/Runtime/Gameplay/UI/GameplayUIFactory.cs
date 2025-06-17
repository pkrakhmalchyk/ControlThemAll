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

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayUIFactory : ILoadingModule, IDisposable
    {
        private readonly IObjectResolver container;

        private GameplayHUDView gameplayHUDPrefab;
        private AsyncOperationHandle<GameObject> gameplayHUDPrefabHandle;
        private GameplayEndLevelUIView gameplayEndLevelUIPrefab;
        private AsyncOperationHandle<GameObject> gameplayEndLevelUIPrefabHandle;
        private GameplayPauseUIView gameplayPauseUIPrefab;
        private AsyncOperationHandle<GameObject> gameplayPauseUIPrefabHandle;
        private RadialSelectionMenuUIView radialSelectionMenuUIPrefab;
        private AsyncOperationHandle<GameObject> radialSelectionMenuUIPrefabHandle;
        private GameplayLevelStartUIView gameplayLevelStartUIPrefab;
        private AsyncOperationHandle<GameObject> gameplayLevelStartUIPrefabHandle;


        public GameplayUIFactory(
            IObjectResolver container)
        {
            this.container = container;
        }

        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public GameplayHUDAdapter CreateGameplayHUD()
        {
            GameplayHUDView gameplayHUDView = Object.Instantiate(gameplayHUDPrefab);
            RadialSelectionMenuUIView radialSelectionMenuUIView = Object.Instantiate(radialSelectionMenuUIPrefab);

            radialSelectionMenuUIView.Initialize(container.Resolve<ITouchInput>());
            radialSelectionMenuUIView.gameObject.SetActive(false);
            gameplayHUDView.Initialize(radialSelectionMenuUIView);
            gameplayHUDView.gameObject.SetActive(false);

            GameplayHUDAdapter gameplayHUDAdapter = new GameplayHUDAdapter(
                gameplayHUDView,
                container.Resolve<UIService>(),
                container.Resolve<InventorySystem<PlayerWeaponConfig>>(),
                container.Resolve<LevelPlayersWeaponsService>(),
                container.Resolve<GameplayLevelController>());

            return gameplayHUDAdapter;
        }

        public GameplayEndLevelUIAdapter CreateGameplayEndLevelUI()
        {
            GameplayEndLevelUIView gameplayEndLevelUIView = Object.Instantiate(gameplayEndLevelUIPrefab);

            gameplayEndLevelUIView.gameObject.SetActive(false);

            GameplayEndLevelUIAdapter gameplayEndLevelUIAdapter = new GameplayEndLevelUIAdapter(
                gameplayEndLevelUIView,
                container.Resolve<UIService>(),
                container.Resolve<GameplayLevelController>(),
                container.Resolve<LevelsDataService>());

            return gameplayEndLevelUIAdapter;
        }

        public GameplayPauseUIAdapter CreateGameplayPauseUI()
        {
            GameplayPauseUIView gameplayPauseUIView = Object.Instantiate(gameplayPauseUIPrefab);

            gameplayPauseUIView.gameObject.SetActive(false);

            GameplayPauseUIAdapter gameplayPauseUIAdapter = new GameplayPauseUIAdapter(
                gameplayPauseUIView,
                container.Resolve<UIService>(),
                container.Resolve<GameplayLevelController>());

            return gameplayPauseUIAdapter;
        }

        public GameplayLevelStartUIAdapter CreateGameplayLevelStartUI()
        {
            GameplayLevelStartUIView gameplayLevelStartUIView = Object.Instantiate(gameplayLevelStartUIPrefab);

            gameplayLevelStartUIView.gameObject.SetActive(false);

            GameplayLevelStartUIAdapter gameplayLevelStartUIAdapter = new GameplayLevelStartUIAdapter(
                gameplayLevelStartUIView,
                container.Resolve<UIService>(),
                container.Resolve<ITouchInput>(),
                container.Resolve<GameplayLevelController>());

            return gameplayLevelStartUIAdapter;
        }


        public void Dispose()
        {
            Addressables.Release(gameplayHUDPrefabHandle);
            Addressables.Release(gameplayEndLevelUIPrefabHandle);
            Addressables.Release(gameplayPauseUIPrefabHandle);
            Addressables.Release(gameplayLevelStartUIPrefabHandle);
        }


        private async UniTask LoadPrefabs()
        {
            gameplayHUDPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.UI.AddresablesGameplayUIPath}/{RuntimeConstants.UI.GameplayHUDWindowPrefabName}");
            gameplayEndLevelUIPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.UI.AddresablesGameplayUIPath}/{RuntimeConstants.UI.GameplayEndLevelPopupPrefabName}");
            gameplayPauseUIPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.UI.AddresablesGameplayUIPath}/{RuntimeConstants.UI.GameplayPausePopupPrefabName}");
            radialSelectionMenuUIPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.UI.AddresablesGameplayUIPath}/{RuntimeConstants.UI.RadialSelectionMenuPrefabName}");
            gameplayLevelStartUIPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.UI.AddresablesGameplayUIPath}/{RuntimeConstants.UI.GameplayLevelStartWindowPrefabName}");

            (GameObject gameplayHUDPrefabGo,
            GameObject gameplayEndLevelUIPrefabGo,
            GameObject gameplayPauseUIPrefabGo,
            GameObject radialSelectionMenuPrefabGo,
            GameObject gameplayLevelStartUIPrefabGo) = await (
                gameplayHUDPrefabHandle.ToUniTask(),
                gameplayEndLevelUIPrefabHandle.ToUniTask(),
                gameplayPauseUIPrefabHandle.ToUniTask(),
                radialSelectionMenuUIPrefabHandle.ToUniTask(),
                gameplayLevelStartUIPrefabHandle.ToUniTask());


            gameplayHUDPrefab = gameplayHUDPrefabGo.GetComponent<GameplayHUDView>();
            gameplayEndLevelUIPrefab = gameplayEndLevelUIPrefabGo.GetComponent<GameplayEndLevelUIView>();
            gameplayPauseUIPrefab = gameplayPauseUIPrefabGo.GetComponent<GameplayPauseUIView>();
            radialSelectionMenuUIPrefab = radialSelectionMenuPrefabGo.GetComponent<RadialSelectionMenuUIView>();
            gameplayLevelStartUIPrefab = gameplayLevelStartUIPrefabGo.GetComponent<GameplayLevelStartUIView>();
        }
    }
}