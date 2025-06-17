using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.MainMenu.UI
{
    public class MainMenuUIView : BaseWindow
    {
        public Action SelectLevelPressed;
        public Action QuitPressed;


        [SerializeField] private RectTransform mainMenuTitle;
        [SerializeField] private Button selectLevelButton;
        [SerializeField] private Button quitButton;


        private MotionHandle showHideHandle;
        private Vector3 animationPositionOffset = new Vector3(Screen.width, 0, 0);
        private Ease animationEase = Ease.OutExpo;
        private Vector3 selectLevelButtonLocalPosition;
        private Vector3 quitButtonLocalPosition;
        private Vector3 mainMenuTitleLocalPosition;


        private void Awake()
        {
            selectLevelButtonLocalPosition = selectLevelButton.transform.localPosition;
            quitButtonLocalPosition = quitButton.transform.localPosition;
            mainMenuTitleLocalPosition = mainMenuTitle.transform.localPosition;
        }

        private void OnDestroy()
        {
            showHideHandle.TryComplete();
        }


        public override async UniTask Show()
        {
            selectLevelButton.onClick.AddListener(OnSelectButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);

            await base.Show();
            await PlayShowAnimation();
        }

        public override async UniTask Hide()
        {
            selectLevelButton.onClick.RemoveListener(OnSelectButtonClicked);
            quitButton.onClick.RemoveListener(OnQuitButtonClicked);

            await PlayHideAnimation();
            await base.Hide();
        }


        private void OnSelectButtonClicked()
        {
            SelectLevelPressed?.Invoke();
        }

        private void OnQuitButtonClicked()
        {
            QuitPressed?.Invoke();
        }

        private async UniTask PlayShowAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(quitButtonLocalPosition + animationPositionOffset, quitButtonLocalPosition, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(quitButton.transform))
                .Join(LMotion.Create(selectLevelButtonLocalPosition - animationPositionOffset, selectLevelButtonLocalPosition, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(selectLevelButton.transform))
                .Join(LMotion.Create(mainMenuTitleLocalPosition + animationPositionOffset, mainMenuTitleLocalPosition, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(mainMenuTitle.transform))
                .Run();

            await showHideHandle;
        }

        private async UniTask PlayHideAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(quitButtonLocalPosition, quitButtonLocalPosition - animationPositionOffset, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(quitButton.transform))
                .Join(LMotion.Create(selectLevelButtonLocalPosition, selectLevelButtonLocalPosition + animationPositionOffset, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(selectLevelButton.transform))
                .Join(LMotion.Create(mainMenuTitleLocalPosition, mainMenuTitleLocalPosition - animationPositionOffset, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(mainMenuTitle.transform))
                .Run();

            await showHideHandle;
        }
    }
}
