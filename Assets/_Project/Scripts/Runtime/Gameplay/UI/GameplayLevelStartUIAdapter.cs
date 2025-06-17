using System;
using System.Threading;
using ControllThemAll.Runtime.Bootstrap.UI;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayLevelStartUIAdapter : IDisposable
    {
        private readonly GameplayLevelStartUIView gameplayLevelStartWindow;
        private readonly UIService uiService;
        private readonly ITouchInput touchInput;
        private readonly GameplayLevelController gameplayLevelController;

        private CancellationTokenSource cts;


        public GameplayLevelStartUIAdapter(
            GameplayLevelStartUIView gameplayLevelStartWindow,
            UIService uiService,
            ITouchInput touchInput,
            GameplayLevelController gameplayLevelController)
        {
            this.gameplayLevelStartWindow = gameplayLevelStartWindow;
            this.uiService = uiService;
            this.touchInput = touchInput;
            this.gameplayLevelController = gameplayLevelController;
        }


        public void InitGameplayStartLevelUI()
        {
            uiService.AddWindow(typeof(GameplayLevelStartUIView), gameplayLevelStartWindow);

            gameplayLevelStartWindow.Activated += OnViewActivated;
            gameplayLevelStartWindow.Deactivated += OnViewDeactivated;
        }

        public void Dispose()
        {
            gameplayLevelStartWindow.Activated -= OnViewActivated;
            gameplayLevelStartWindow.Deactivated -= OnViewDeactivated;

            uiService.RemoveWindow(typeof(GameplayLevelStartUIView));

            if (gameplayLevelStartWindow != null)
            {
                UnityEngine.Object.Destroy(gameplayLevelStartWindow.gameObject);
            }

            cts?.Dispose();
        }


        private void OnViewActivated()
        {
            cts = new CancellationTokenSource();

            StartGameplayAfterTouch(cts.Token).Forget();
        }

        private void OnViewDeactivated()
        {
            cts?.Cancel();
            cts?.Dispose();

            cts = null;
        }

        private async UniTask StartGameplayAfterTouch(CancellationToken cancellationToken)
        {
            while (!touchInput.IsActive)
            {
                await UniTask.Yield(cancellationToken);
            }

            gameplayLevelController.StartGameplay();
            uiService.SetWindowVisible(typeof(GameplayHUDView), true).Forget();
        }
    }
}