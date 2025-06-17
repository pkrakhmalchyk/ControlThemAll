using System.Threading;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace ControllThemAll.Runtime.Bootstrap
{
    public class BootstrapEntryPoint : IAsyncStartable
    {
        private readonly int nextSceneIndex;
        private readonly ModulesLoader modulesLoader;
        private readonly RootUIFactory rootUIFactory;
        private readonly UIService uiService;
        private readonly LevelsDataService levelsDataService;


        public BootstrapEntryPoint(
            int nextSceneIndex,
            ModulesLoader modulesLoader,
            RootUIFactory rootUIFactory,
            UIService uiService,
            LevelsDataService levelsDataService)
        {
            this.nextSceneIndex = nextSceneIndex;
            this.modulesLoader = modulesLoader;
            this.rootUIFactory = rootUIFactory;
            this.uiService = uiService;
            this.levelsDataService = levelsDataService;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;

            await modulesLoader.Load(rootUIFactory);
            await modulesLoader.Load(uiService);

            uiService.SetLoadingWindowVisible(true);

            await modulesLoader.Load(levelsDataService);

            if (nextSceneIndex == RuntimeConstants.Scenes.MainMenu)
            {
                await ScenesLoader.LoadScene(RuntimeConstants.Scenes.MainMenu);
            }
            else if (nextSceneIndex == RuntimeConstants.Scenes.Gameplay)
            {
                await ScenesLoader.LoadScene(RuntimeConstants.Scenes.Gameplay);
            }
            else
            {
                await ScenesLoader.LoadScene(RuntimeConstants.Scenes.MainMenu);
            }

            uiService.SetLoadingWindowVisible(false);
        }
    }
}
