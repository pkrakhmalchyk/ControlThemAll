using System.Threading;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.MainMenu.UI;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace ControllThemAll.Runtime.MainMenu
{
    public class MainMenuEntryPoint : IAsyncStartable
    {
        private readonly ModulesLoader modulesLoader;
        private readonly MainMenuUIFactory mainMenuUIFactory;
        private readonly MainMenuUIInstaller mainMenuUIInstaller;
        private readonly UIService uiService;


        public MainMenuEntryPoint(
            ModulesLoader modulesLoader,
            MainMenuUIFactory mainMenuUIFactory,
            MainMenuUIInstaller mainMenuUIInstaller,
            UIService uiService)
        {
            this.modulesLoader = modulesLoader;
            this.mainMenuUIFactory = mainMenuUIFactory;
            this.mainMenuUIInstaller = mainMenuUIInstaller;
            this.uiService = uiService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            uiService.SetLoadingWindowVisible(true);

            await modulesLoader.Load(mainMenuUIFactory);
            await modulesLoader.Load(mainMenuUIInstaller);

            uiService.SetLoadingWindowVisible(false);

            mainMenuUIInstaller.ShowMainMenuUI();
        }
    }
}
