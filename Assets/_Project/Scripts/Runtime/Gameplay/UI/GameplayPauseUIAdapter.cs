using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayPauseUIAdapter : IDisposable
    {
        private readonly GameplayPauseUIView gameplayPauseUIPopup;
        private readonly UIService uiService;
        private readonly GameplayLevelController gameplayLevelController;


        public GameplayPauseUIAdapter(
            GameplayPauseUIView gameplayPauseUIPopup,
            UIService uiService,
            GameplayLevelController gameplayLevelController)
        {
            this.gameplayPauseUIPopup = gameplayPauseUIPopup;
            this.uiService = uiService;
            this.gameplayLevelController = gameplayLevelController;
        }

        public void InitGameplayPauseUI()
        {
            uiService.AddPopup(typeof(GameplayPauseUIView), gameplayPauseUIPopup);

            gameplayPauseUIPopup.Activated += OnViewActivated;
            gameplayPauseUIPopup.Deactivated += OnViewDeactivated;
        }

        public void Dispose()
        {
            gameplayPauseUIPopup.Activated -= OnViewActivated;
            gameplayPauseUIPopup.Deactivated -= OnViewDeactivated;

            DetachViewEvents();
            uiService.RemovePopup(typeof(GameplayPauseUIView));

            if (gameplayPauseUIPopup != null)
            {
                UnityEngine.Object.Destroy(gameplayPauseUIPopup.gameObject);
            }
        }


        private void OnViewActivated()
        {
            AttachViewEvents();
            gameplayLevelController.PauseGameplay();
        }

        private void OnViewDeactivated()
        {
            DetachViewEvents();
            gameplayLevelController.ResumeGameplay();
        }

        private void AttachViewEvents()
        {
            gameplayPauseUIPopup.MainMenuButtonPressed += OnMainMenuButtonPressed;
            gameplayPauseUIPopup.ResumeButtonPressed += OnResumeButtonPressed;
            gameplayPauseUIPopup.RestartLevelButtonPressed += OnRestartLevelButtonPressed;
        }

        private void DetachViewEvents()
        {
            gameplayPauseUIPopup.MainMenuButtonPressed -= OnMainMenuButtonPressed;
            gameplayPauseUIPopup.ResumeButtonPressed -= OnResumeButtonPressed;
            gameplayPauseUIPopup.RestartLevelButtonPressed -= OnRestartLevelButtonPressed;
        }

        private void OnResumeButtonPressed()
        {
            uiService.SetPopupVisible(typeof(GameplayPauseUIView), false).Forget();
        }

        private void OnMainMenuButtonPressed()
        {
            ScenesLoader.LoadScene(RuntimeConstants.Scenes.MainMenu).Forget();
        }

        private void OnRestartLevelButtonPressed()
        {
            ScenesLoader.LoadScene(RuntimeConstants.Scenes.Gameplay).Forget();
        }
    }
}