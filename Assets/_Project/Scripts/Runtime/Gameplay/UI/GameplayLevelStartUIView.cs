using System;
using ControllThemAll.Runtime.Bootstrap.UI;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay.UI
{
    public class GameplayLevelStartUIView : BaseWindow
    {
        [SerializeField] private TMP_Text levelStartText;
        [SerializeField] private Ease animationEase = Ease.Linear;
        [SerializeField] private float animationScaleMultiplier = 0.2f;
        [SerializeField] private float animationDuration = 0.2f;


        private MotionHandle showHideHandle;
        private Vector3 initialScale;


        private void Awake()
        {
            initialScale = levelStartText.transform.localScale;
        }

        private void OnEnable()
        {
            showHideHandle = LMotion.Create(initialScale, initialScale * animationScaleMultiplier, animationDuration)
                .WithLoops(-1, LoopType.Yoyo)
                .WithEase(animationEase)
                .BindToLocalScale(levelStartText.transform);
        }

        private void OnDisable()
        {
            showHideHandle.TryCancel();
        }
    }
}