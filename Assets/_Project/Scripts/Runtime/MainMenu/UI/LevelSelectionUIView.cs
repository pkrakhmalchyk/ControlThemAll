using System;
using System.Collections.Generic;
using System.Linq;
using ControllThemAll.Runtime.Bootstrap.UI;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ControllThemAll.Runtime.MainMenu.UI
{

    public class LevelSelectionUIView : BaseWindow
    {
        public Action<string> LevelSelected;
        public Action BackButtonPressed;


        [SerializeField] private RectTransform levelSelectionTitle;
        [SerializeField] private Transform levelElementsContainer;
        [SerializeField] private LevelUIElementView levelElementViewPrefab;
        [SerializeField] private Button backButton;


        private readonly List<LevelUIElementView> levelsElements = new();
        private MotionHandle showHideHandle;
        private Vector3 animationPositionOffset = new Vector3(Screen.width, 0, 0);
        private Ease animationEase = Ease.OutExpo;
        private Vector3 levelSelectionTitleLocalPosition;
        private Vector3 levelElementsContainerLocalPosition;
        private Vector3 backButtonLocalPosition;


        private void Awake()
        {
            levelSelectionTitleLocalPosition = levelSelectionTitle.transform.localPosition;
            levelElementsContainerLocalPosition = levelElementsContainer.transform.localPosition;
            backButtonLocalPosition = backButton.transform.localPosition;
        }

        private void OnDestroy()
        {
            showHideHandle.TryComplete();
        }


        public override async UniTask Show()
        {
            backButton.onClick.AddListener(OnBackButtonPressed);

            foreach (LevelUIElementView levelElement in levelsElements)
            {
                levelElement.Selected += OnLevelSelected;
            }

            await base.Show();
            await PlayShowAnimation();
        }

        public override async UniTask Hide()
        {
            backButton.onClick.RemoveListener(OnBackButtonPressed);

            foreach (LevelUIElementView levelElement in levelsElements)
            {
                levelElement.Selected -= OnLevelSelected;
            }

            await PlayHideAnimation();
            await base.Hide();
        }

        public void SetLevels(IReadOnlyCollection<LevelConfig> levelsConfigs, int currentLevel)
        {
            for (int i = 0; i < levelsConfigs.Count; i++)
            {
                LevelUIElementView levelUIElementView = Instantiate(levelElementViewPrefab, levelElementsContainer, false);

                levelUIElementView.SetKey(levelsConfigs.ElementAt(i).LevelName);
                levelUIElementView.SetLevelText(levelsConfigs.ElementAt(i).LevelName);
                levelUIElementView.SetLevelOpen(i <= currentLevel);

                levelsElements.Add(levelUIElementView);
            }
        }


        private void OnLevelSelected(LevelUIElementView levelElementView)
        {
            LevelSelected?.Invoke(levelElementView.Key);
        }

        private void OnBackButtonPressed()
        {
            BackButtonPressed?.Invoke();
        }

        private async UniTask PlayShowAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(backButtonLocalPosition + animationPositionOffset, backButtonLocalPosition, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(backButton.transform))
                .Join(LMotion.Create(levelElementsContainerLocalPosition - animationPositionOffset, levelElementsContainerLocalPosition, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(levelElementsContainer.transform))
                .Join(LMotion.Create(levelSelectionTitleLocalPosition + animationPositionOffset, levelSelectionTitleLocalPosition, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(levelSelectionTitle.transform))
                .Run();

            await showHideHandle;
        }

        private async UniTask PlayHideAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(backButtonLocalPosition, backButtonLocalPosition - animationPositionOffset, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(backButton.transform))
                .Join(LMotion.Create(levelElementsContainerLocalPosition, levelElementsContainerLocalPosition + animationPositionOffset, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(levelElementsContainer.transform))
                .Join(LMotion.Create(levelSelectionTitleLocalPosition, levelSelectionTitleLocalPosition - animationPositionOffset, 0.2f)
                    .WithEase(animationEase)
                    .BindToLocalPosition(levelSelectionTitle.transform))
                .Run();;

            await showHideHandle;
        }
    }
}