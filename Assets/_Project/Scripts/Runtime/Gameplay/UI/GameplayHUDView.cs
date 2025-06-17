using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayHUDView : BaseWindow
    {
        public Action<string> PlayerWeaponSelected;
        public Action PlayerWeaponMenuOpened;
        public Action PlayerWeaponMenuClosed;
        public Action PauseButtonPressed;


        [SerializeField] private Transform playerWeaponsElementsContainer;
        [SerializeField] private Button pauseButton;

        private RadialSelectionMenuUIView radialWeaponSelectionUIView;
        private MotionHandle showHideHandle;
        private Vector3 animationPositionVerticalOffset = new Vector3(0, Screen.height, 0);
        private Ease animationEase = Ease.OutExpo;
        private Vector3 pauseButtonLocalPosition;
        private Vector3 playerWeaponsElementsContainerLocalPosition;


        private void Awake()
        {
            pauseButtonLocalPosition = pauseButton.transform.localPosition;
            playerWeaponsElementsContainerLocalPosition = playerWeaponsElementsContainer.localPosition;
        }

        private void OnDestroy()
        {
            showHideHandle.TryComplete();
        }


        public void Initialize(RadialSelectionMenuUIView radialWeaponSelectionUIView)
        {
            this.radialWeaponSelectionUIView = radialWeaponSelectionUIView;
            Transform radialWeaponSelectionTransform = this.radialWeaponSelectionUIView.transform;

            radialWeaponSelectionTransform.SetParent(playerWeaponsElementsContainer);
            radialWeaponSelectionTransform.position = playerWeaponsElementsContainer.position;
            radialWeaponSelectionUIView.gameObject.SetActive(true);
        }

        public override async UniTask Show()
        {
            pauseButton.onClick.AddListener(OnPauseButtonPressed);

            if (radialWeaponSelectionUIView != null)
            {
                radialWeaponSelectionUIView.Selected += OnPlayerWeaponSelected;
                radialWeaponSelectionUIView.Opened += OnPlayerWeaponMenuOpened;
                radialWeaponSelectionUIView.Closed += OnPlayerWeaponMenuClosed;
            }

            await base.Show();
            await PlayShowAnimation();
        }

        public override async UniTask Hide()
        {
            pauseButton.onClick.RemoveListener(OnPauseButtonPressed);

            if (radialWeaponSelectionUIView != null)
            {
                radialWeaponSelectionUIView.Selected -= OnPlayerWeaponSelected;
                radialWeaponSelectionUIView.Opened -= OnPlayerWeaponMenuOpened;
                radialWeaponSelectionUIView.Closed -= OnPlayerWeaponMenuClosed;
            }

            await PlayHideAnimation();
            await base.Hide();
        }


        public void AddPlayerWeaponUIElement(string key, Sprite icon, int count)
        {
            radialWeaponSelectionUIView.AddRadialSelectionMenuUIElement(key, icon, count);
        }

        public void RemovePlayerWeaponUIElement(string key)
        {
            radialWeaponSelectionUIView.RemoveRadialSelectionMenuUIElement(key);
        }

        public void RemoveAllPlayersWeaponsUIElements()
        {
            radialWeaponSelectionUIView.RemoveAllRadialSelectionMenuUIElements();
        }

        public void SetPlayerWeaponUIElementCount(string key, int count)
        {
            radialWeaponSelectionUIView.SetRadialSelectionMenuUIElementCount(key, count);
        }

        public void SetPlayerWeaponUIActiveElement(string key)
        {
            radialWeaponSelectionUIView.SetRadialSelectionMenuUIActiveElement(key);
        }


        private void OnPauseButtonPressed()
        {
            PauseButtonPressed?.Invoke();
        }

        private void OnPlayerWeaponSelected(string key)
        {
            PlayerWeaponSelected?.Invoke(key);
        }

        private void OnPlayerWeaponMenuOpened()
        {
            PlayerWeaponMenuOpened?.Invoke();
        }

        private void OnPlayerWeaponMenuClosed()
        {
            PlayerWeaponMenuClosed?.Invoke();
        }

        private async UniTask PlayShowAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(pauseButtonLocalPosition + animationPositionVerticalOffset, pauseButtonLocalPosition, 1f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(pauseButton.transform))
                .Join(LMotion.Create(playerWeaponsElementsContainerLocalPosition - animationPositionVerticalOffset, playerWeaponsElementsContainerLocalPosition, 1f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(playerWeaponsElementsContainer))
                .Run();

            await showHideHandle;
        }

        private async UniTask PlayHideAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(pauseButtonLocalPosition, pauseButtonLocalPosition + animationPositionVerticalOffset, 1f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(pauseButton.transform))
                .Join(LMotion.Create(playerWeaponsElementsContainerLocalPosition, playerWeaponsElementsContainerLocalPosition - animationPositionVerticalOffset, 1f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(playerWeaponsElementsContainer))
                .Run();

            await showHideHandle;
        }
    }
}