using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayHUDAdapter : IDisposable
    {
        private readonly GameplayHUDView gameplayHUDView;
        private readonly UIService uiService;
        private readonly InventorySystem<PlayerWeaponConfig> inventorySystem;
        private readonly LevelPlayersWeaponsService levelPlayersWeaponsService;
        private readonly GameplayLevelController gameplayLevelController;


        public GameplayHUDAdapter(
            GameplayHUDView gameplayHUDView,
            UIService uiService,
            InventorySystem<PlayerWeaponConfig> inventorySystem,
            LevelPlayersWeaponsService levelPlayersWeaponsService,
            GameplayLevelController gameplayLevelController)
        {
            this.gameplayHUDView = gameplayHUDView;
            this.uiService = uiService;
            this.inventorySystem = inventorySystem;
            this.levelPlayersWeaponsService = levelPlayersWeaponsService;
            this.gameplayLevelController = gameplayLevelController;
        }

        public void InitGameplayHUD()
        {
            uiService.AddWindow(typeof(GameplayHUDView), gameplayHUDView);

            gameplayHUDView.Activated += OnViewActivated;
            gameplayHUDView.Deactivated += OnViewDeactivated;
        }

        public void Dispose()
        {
            gameplayHUDView.Activated -= OnViewActivated;
            gameplayHUDView.Deactivated -= OnViewDeactivated;

            DetachViewEvents();
            uiService.RemoveWindow(typeof(GameplayHUDView));

            if (gameplayHUDView != null)
            {
                UnityEngine.Object.Destroy(gameplayHUDView.gameObject);
            }
        }


        private void OnViewActivated()
        {
            AttachViewEvents();
            UpdatePlayersWeaponsUI();
        }

        private void OnViewDeactivated()
        {
            DetachViewEvents();
            gameplayLevelController.ResumeGameplay();
        }

        private void AttachViewEvents()
        {
            gameplayHUDView.PlayerWeaponSelected += OnPlayerWeaponSelected;
            gameplayHUDView.PauseButtonPressed += OnPauseButtonPressed;
            gameplayHUDView.PlayerWeaponMenuOpened += OnPlayerWeaponMenuOpened;
            gameplayHUDView.PlayerWeaponMenuClosed += OnPlayerWeaponMenuClosed;
            inventorySystem.ItemCountChanged += OnItemCountChanged;
            levelPlayersWeaponsService.WeaponChanged += OnPlayersWeaponChanged;
            gameplayLevelController.LevelLost += OnLevelEnd;
            gameplayLevelController.LevelWon += OnLevelEnd;
        }

        private void DetachViewEvents()
        {
            gameplayHUDView.PlayerWeaponSelected -= OnPlayerWeaponSelected;
            gameplayHUDView.PauseButtonPressed -= OnPauseButtonPressed;
            gameplayHUDView.PlayerWeaponMenuOpened -= OnPlayerWeaponMenuOpened;
            gameplayHUDView.PlayerWeaponMenuClosed -= OnPlayerWeaponMenuClosed;
            inventorySystem.ItemCountChanged -= OnItemCountChanged;
            levelPlayersWeaponsService.WeaponChanged -= OnPlayersWeaponChanged;
            gameplayLevelController.LevelLost -= OnLevelEnd;
            gameplayLevelController.LevelWon -= OnLevelEnd;
        }

        private void OnPauseButtonPressed()
        {
            uiService.SetPopupVisible(typeof(GameplayPauseUIView), true).Forget();
        }

        private void OnPlayerWeaponMenuOpened()
        {
            gameplayLevelController.SlowDownGameplay(RuntimeConstants.Settings.GameplaySlowDownSpeed);
        }

        private void OnPlayerWeaponMenuClosed()
        {
            gameplayLevelController.ResumeGameplay();
        }

        private void OnPlayerWeaponSelected(string key)
        {
            PlayerWeaponConfig selectedConfig = null;

            for (int i = 0; i < inventorySystem.Count; i++)
            {
                PlayerWeaponConfig playerWeaponConfig = inventorySystem.GetValue(i);

                if (playerWeaponConfig.name == key)
                {
                    selectedConfig = playerWeaponConfig;
                }
            }

            if (selectedConfig == null)
            {
                return;
            }

            levelPlayersWeaponsService.SetCurrentWeapon(selectedConfig);
        }

        private void OnItemCountChanged(PlayerWeaponConfig playerWeaponConfig)
        {
            UpdatePlayerWeaponUIElement(playerWeaponConfig);
        }

        private void OnPlayersWeaponChanged()
        {
            gameplayHUDView.SetPlayerWeaponUIActiveElement(levelPlayersWeaponsService.CurrentWeapon.name);
        }

        private void OnLevelEnd()
        {
            uiService.SetPopupVisible(typeof(GameplayEndLevelUIView), true).Forget();
        }

        private void UpdatePlayersWeaponsUI()
        {
            gameplayHUDView.RemoveAllPlayersWeaponsUIElements();

            for (int i = 0; i < inventorySystem.Count; i++)
            {
                PlayerWeaponConfig playerWeaponConfig = inventorySystem.GetValue(i);
                int count = inventorySystem.GetCount(playerWeaponConfig);

                gameplayHUDView.AddPlayerWeaponUIElement(playerWeaponConfig.name, playerWeaponConfig.Icon, count);
            }

            gameplayHUDView.SetPlayerWeaponUIActiveElement(levelPlayersWeaponsService.CurrentWeapon.name);
        }

        private void UpdatePlayerWeaponUIElement(PlayerWeaponConfig playerWeaponConfig)
        {
            int count = inventorySystem.GetCount(playerWeaponConfig);

            if (count > 0)
            {
                gameplayHUDView.SetPlayerWeaponUIElementCount(playerWeaponConfig.name, count);
            }
            else
            {
                gameplayHUDView.RemovePlayerWeaponUIElement(playerWeaponConfig.name);
            }
        }
    }
}