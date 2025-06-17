using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayPauseUIView : BasePopup
    {
        public Action ResumeButtonPressed;
        public Action RestartLevelButtonPressed;
        public Action MainMenuButtonPressed;


        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button mainMenuButton;


        public override async UniTask Show()
        {
            resumeButton.onClick.AddListener(OnResumeButtonPressed);
            restartLevelButton.onClick.AddListener(OnRestartLevelButtonPressed);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);

            await base.Show();
        }

        public override async UniTask Hide()
        {
            resumeButton.onClick.RemoveListener(OnResumeButtonPressed);
            restartLevelButton.onClick.RemoveListener(OnRestartLevelButtonPressed);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonPressed);

            await base.Hide();
        }


        private void OnResumeButtonPressed()
        {
            ResumeButtonPressed?.Invoke();
        }

        private void OnRestartLevelButtonPressed()
        {
            RestartLevelButtonPressed?.Invoke();
        }

        private void OnMainMenuButtonPressed()
        {
            MainMenuButtonPressed?.Invoke();
        }
    }
}