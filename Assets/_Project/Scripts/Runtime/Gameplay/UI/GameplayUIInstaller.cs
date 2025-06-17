using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayUIInstaller : ILoadingModule, IDisposable
    {
        private readonly GameplayUIFactory gameplayUIFactory;
        private readonly UIService uiService;

        private GameplayEndLevelUIAdapter gameplayEndLevelUIAdapter;
        private GameplayHUDAdapter gameplayHUDAdapter;
        private GameplayPauseUIAdapter gameplayPauseUIAdapter;
        private GameplayLevelStartUIAdapter gameplayLevelStartUIAdapter;


        public GameplayUIInstaller(
            GameplayUIFactory gameplayUIFactory,
            UIService uiService)
        {
            this.gameplayUIFactory = gameplayUIFactory;
            this.uiService = uiService;
        }

        public UniTask Load()
        {
            gameplayEndLevelUIAdapter = gameplayUIFactory.CreateGameplayEndLevelUI();
            gameplayHUDAdapter = gameplayUIFactory.CreateGameplayHUD();
            gameplayPauseUIAdapter = gameplayUIFactory.CreateGameplayPauseUI();
            gameplayLevelStartUIAdapter = gameplayUIFactory.CreateGameplayLevelStartUI();

            gameplayEndLevelUIAdapter.InitGameplayEndLevelUI();
            gameplayHUDAdapter.InitGameplayHUD();
            gameplayPauseUIAdapter.InitGameplayPauseUI();
            gameplayLevelStartUIAdapter.InitGameplayStartLevelUI();

            return UniTask.CompletedTask;
        }

        public void ShowGameplayLevelInitialUI()
        {
            uiService.SetWindowVisible(typeof(GameplayLevelStartUIView), true).Forget();
        }

        public void Dispose()
        {
            gameplayEndLevelUIAdapter.Dispose();
            gameplayHUDAdapter.Dispose();
            gameplayPauseUIAdapter.Dispose();
            gameplayLevelStartUIAdapter.Dispose();
        }
    }
}