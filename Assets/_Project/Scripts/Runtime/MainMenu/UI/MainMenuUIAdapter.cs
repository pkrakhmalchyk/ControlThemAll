using ControllThemAll.Runtime.Bootstrap.UI;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.MainMenu.UI
{
    public class MainMenuUIAdapter : IDisposable
    {
        private readonly MainMenuUIView mainMenuUIView;
        private readonly UIService uiService;


        public MainMenuUIAdapter(MainMenuUIView mainMenuUIView, UIService uiService)
        {
            this.mainMenuUIView = mainMenuUIView;
            this.uiService = uiService;
        }


        public void InitMainMenuUI()
        {
            uiService.AddWindow(typeof(MainMenuUIView), mainMenuUIView);

            mainMenuUIView.Activated += OnViewActivated;
            mainMenuUIView.Deactivated += OnViewDeactivated;
        }

        public void Dispose()
        {
            mainMenuUIView.Activated -= OnViewActivated;
            mainMenuUIView.Deactivated -= OnViewDeactivated;

            DetachViewEvents();
            uiService.RemoveWindow(typeof(MainMenuUIView));

            if (mainMenuUIView != null)
            {
                UnityEngine.Object.Destroy(mainMenuUIView.gameObject);
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
            mainMenuUIView.SelectLevelPressed += OnSelectLevelPressed;
            mainMenuUIView.QuitPressed += OnQuitPressed;
        }

        private void DetachViewEvents()
        {
            mainMenuUIView.SelectLevelPressed -= OnSelectLevelPressed;
            mainMenuUIView.QuitPressed -= OnQuitPressed;
        }

        private void OnSelectLevelPressed()
        {
            uiService.SetWindowVisible(typeof(LevelSelectionUIView), true).Forget();
        }

        private void OnQuitPressed()
        {
            Application.Quit();
        }
    }
}
