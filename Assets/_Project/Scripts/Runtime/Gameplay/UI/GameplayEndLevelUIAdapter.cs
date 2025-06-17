using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayEndLevelUIAdapter : IDisposable
    {
        private readonly GameplayEndLevelUIView gameplayEndLevelUIPopup;
        private readonly UIService uiService;
        private readonly GameplayLevelController gameplayLevelController;
        private readonly LevelsDataService levelsDataService;


        public GameplayEndLevelUIAdapter(
            GameplayEndLevelUIView gameplayEndLevelUIPopup,
            UIService uiService,
            GameplayLevelController gameplayLevelController,
            LevelsDataService levelsDataService)
        {
            this.gameplayEndLevelUIPopup = gameplayEndLevelUIPopup;
            this.uiService = uiService;
            this.gameplayLevelController = gameplayLevelController;
            this.levelsDataService = levelsDataService;
        }

        public void InitGameplayEndLevelUI()
        {
            uiService.AddPopup(typeof(GameplayEndLevelUIView), gameplayEndLevelUIPopup);

            gameplayEndLevelUIPopup.Activated += OnViewActivated;
            gameplayEndLevelUIPopup.Deactivated += OnViewDeactivated;
        }

        public void Dispose()
        {
            gameplayEndLevelUIPopup.Activated -= OnViewActivated;
            gameplayEndLevelUIPopup.Deactivated -= OnViewDeactivated;

            DetachViewEvents();
            uiService.RemovePopup(typeof(GameplayEndLevelUIView));

            if (gameplayEndLevelUIPopup != null)
            {
                UnityEngine.Object.Destroy(gameplayEndLevelUIPopup.gameObject);
            }
        }


        private void OnViewActivated()
        {
            AttachViewEvents();

            if (gameplayLevelController.State == LevelState.Win)
            {
                gameplayEndLevelUIPopup.SetNextLevelButtonVisible(NextLevelExists());
                gameplayEndLevelUIPopup.SetEndLevelText("Win");
            }
            else if (gameplayLevelController.State == LevelState.Defeat)
            {
                gameplayEndLevelUIPopup.SetNextLevelButtonVisible(false);
                gameplayEndLevelUIPopup.SetEndLevelText("Game Over");
            }

            gameplayLevelController.PauseGameplay();
        }

        private void OnViewDeactivated()
        {
            DetachViewEvents();
            gameplayLevelController.ResumeGameplay();
        }

        private void AttachViewEvents()
        {
            gameplayEndLevelUIPopup.NextLevelButtonPressed += OnNextLevelButtonPressed;
            gameplayEndLevelUIPopup.RestartLevelButtonPressed += OnRestartLevelButtonPressed;
            gameplayEndLevelUIPopup.MainMenuButtonPressed += OnMainMenuButtonPressed;
        }

        private void DetachViewEvents()
        {
            gameplayEndLevelUIPopup.NextLevelButtonPressed -= OnNextLevelButtonPressed;
            gameplayEndLevelUIPopup.RestartLevelButtonPressed -= OnRestartLevelButtonPressed;
            gameplayEndLevelUIPopup.MainMenuButtonPressed -= OnMainMenuButtonPressed;
        }

        private void OnMainMenuButtonPressed()
        {
            ScenesLoader.LoadScene(RuntimeConstants.Scenes.MainMenu).Forget();
        }

        private void OnRestartLevelButtonPressed()
        {
            ScenesLoader.LoadScene(RuntimeConstants.Scenes.Gameplay).Forget();
        }

        private void OnNextLevelButtonPressed()
        {
            int currentLevelIndex = levelsDataService.GetCurrentLevelIndex();
            levelsDataService.SetCurrentLevelByIndex(currentLevelIndex + 1);

            ScenesLoader.LoadScene(RuntimeConstants.Scenes.Gameplay).Forget();
        }

        private bool NextLevelExists()
        {
            int currentLevelIndex = levelsDataService.GetCurrentLevelIndex();

            return currentLevelIndex < levelsDataService.GetAllLevelConfigs().Count - 1;
        }
    }
}