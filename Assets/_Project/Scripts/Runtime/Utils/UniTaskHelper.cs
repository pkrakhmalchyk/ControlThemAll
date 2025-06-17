using System.Threading;
using ControllThemAll.Runtime.Gameplay;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.Utils
{
    public static class UniTaskHelper
    {
        public static async UniTask DelayWithGameplayTimeScale(float delay, TimeScaleHolder timeScaleHolder, CancellationToken cancellationToken = default)
        {
            float passedTime = 0f;

            while (passedTime < delay && !cancellationToken.IsCancellationRequested)
            {
                passedTime += Time.deltaTime * timeScaleHolder.GameplayTimeScale;

                await UniTask.Yield();
            }
        }

        public static async UniTask DelayWithGameplayVisualEffectsTimeScale(float delay, TimeScaleHolder timeScaleHolder, CancellationToken cancellationToken = default)
        {
            float passedTime = 0f;

            while (passedTime < delay && !cancellationToken.IsCancellationRequested)
            {
                passedTime += Time.deltaTime * timeScaleHolder.GameplayVisualEffectsTimeScale;

                await UniTask.Yield();
            }
        }
    }
}