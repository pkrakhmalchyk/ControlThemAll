using System;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class TimeScaleHolder
    {
        public Action GameplayTimeScaleChanged;
        public Action GameplayVisualEffectsTimeScaleChanged;


        private float gameplayTimeScale = 1.0f;
        private float gameplayVisualEffectsTimeScale = 1.0f;


        public float GameplayTimeScale => gameplayTimeScale;
        public float GameplayVisualEffectsTimeScale => gameplayVisualEffectsTimeScale;


        public void SetGameplayTimeScale(float gameplayTimeScale)
        {
            this.gameplayTimeScale = Mathf.Clamp01(gameplayTimeScale);

            GameplayTimeScaleChanged?.Invoke();
        }

        public void SetGameplayVisualEffectsTimeScale(float gameplayVisualEffectsTimeScale)
        {
            this.gameplayVisualEffectsTimeScale = Mathf.Clamp01(gameplayVisualEffectsTimeScale);

            GameplayVisualEffectsTimeScaleChanged?.Invoke();
        }
    }
}