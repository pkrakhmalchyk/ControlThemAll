using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayEndLevelUIView : BasePopup
    {
        public Action NextLevelButtonPressed;
        public Action RestartLevelButtonPressed;
        public Action MainMenuButtonPressed;


        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private TMP_Text endLevelText;

        public override async UniTask Show()
        {
            nextLevelButton.onClick.AddListener(OnNextLevelButtonPressed);
            restartLevelButton.onClick.AddListener(OnRestartLevelButtonPressed);
            mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);

            await base.Show();
        }

        public override async UniTask Hide()
        {
            nextLevelButton.onClick.RemoveListener(OnNextLevelButtonPressed);
            restartLevelButton.onClick.RemoveListener(OnRestartLevelButtonPressed);
            mainMenuButton.onClick.RemoveListener(OnMainMenuButtonPressed);

            await base.Hide();
        }

        public void SetEndLevelText(string text)
        {
            endLevelText.text = text;
        }

        public void SetNextLevelButtonVisible(bool visible)
        {
            nextLevelButton.gameObject.SetActive(visible);
        }


        private void OnNextLevelButtonPressed()
        {
            NextLevelButtonPressed?.Invoke();
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