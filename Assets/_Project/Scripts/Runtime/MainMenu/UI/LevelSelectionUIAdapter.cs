using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.MainMenu.UI
{
    public class LevelSelectionUIAdapter : IDisposable
    {
        private readonly LevelSelectionUIView levelSelectionUIView;
        private readonly UIService uiService;
        private readonly IDataHandler dataHandler;
        private readonly LevelsDataService levelsDataService;


        public LevelSelectionUIAdapter(
            LevelSelectionUIView levelSelectionUIView,
            UIService uiService,
            IDataHandler dataHandler,
            LevelsDataService levelsDataService)
        {
            this.levelSelectionUIView = levelSelectionUIView;
            this.uiService = uiService;
            this.dataHandler = dataHandler;
            this.levelsDataService = levelsDataService;
        }

        public async UniTask InitLevelSelectionUI()
        {
            GameProgressData gameProgressData = await dataHandler.Load<GameProgressData>(RuntimeConstants.Data.GameProgressData);

            uiService.AddWindow(typeof(LevelSelectionUIView), levelSelectionUIView);
            levelSelectionUIView.SetLevels(levelsDataService.GetAllLevelConfigs(), gameProgressData?.CurrentLevel ?? 0);

            levelSelectionUIView.Activated += OnViewActivated;
            levelSelectionUIView.Deactivated += OnViewDeactivated;
        }

        public void Dispose()
        {
            levelSelectionUIView.Activated -= OnViewActivated;
            levelSelectionUIView.Deactivated -= OnViewDeactivated;

            DetachViewEvents();
            uiService.RemoveWindow(typeof(LevelSelectionUIView));

            if (levelSelectionUIView != null)
            {
                UnityEngine.Object.Destroy(levelSelectionUIView.gameObject);
            }
        }


        private void OnViewActivated()
        {
            AttachViewEvents();
        }

        private void OnViewDeactivated()
        {
            DetachViewEvents();
        }

        private void AttachViewEvents()
        {
            levelSelectionUIView.LevelSelected += OnLevelSelected;
            levelSelectionUIView.BackButtonPressed += OnBackButtonPressed;
        }

        private void DetachViewEvents()
        {
            levelSelectionUIView.LevelSelected -= OnLevelSelected;
            levelSelectionUIView.BackButtonPressed -= OnBackButtonPressed;
        }

        private void OnLevelSelected(string levelName)
        {
            levelsDataService.SetCurrentLevelByName(levelName);

            ScenesLoader.LoadScene(RuntimeConstants.Scenes.Gameplay).Forget();
        }

        private void OnBackButtonPressed()
        {
            uiService.SetWindowVisible(typeof(MainMenuUIView), true).Forget();
        }
    }
}