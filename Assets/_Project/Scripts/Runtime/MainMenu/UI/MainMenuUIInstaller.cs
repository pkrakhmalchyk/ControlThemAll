using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.MainMenu.UI
{
    public class MainMenuUIInstaller : ILoadingModule, IDisposable
    {
        private readonly MainMenuUIFactory mainMenuUIFactory;
        private readonly UIService uiService;

        private MainMenuUIAdapter mainMenuUIAdapter;
        private LevelSelectionUIAdapter levelSelectionUIAdapter;


        public MainMenuUIInstaller(
            MainMenuUIFactory mainMenuUIFactory,
            UIService uiService)
        {
            this.mainMenuUIFactory = mainMenuUIFactory;
            this.uiService = uiService;
        }

        public async UniTask Load()
        {
            mainMenuUIAdapter = mainMenuUIFactory.CreateMainMenuUI();
            levelSelectionUIAdapter = mainMenuUIFactory.CreateLevelSelectionUI();

            mainMenuUIAdapter.InitMainMenuUI();
            await levelSelectionUIAdapter.InitLevelSelectionUI();
        }

        public void ShowMainMenuUI()
        {
            uiService.SetWindowVisible(typeof(MainMenuUIView), true).Forget();
        }

        public void Dispose()
        {
            mainMenuUIAdapter.Dispose();
            levelSelectionUIAdapter.Dispose();
        }
    }
}