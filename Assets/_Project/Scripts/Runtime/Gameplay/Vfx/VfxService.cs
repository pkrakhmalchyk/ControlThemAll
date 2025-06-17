using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class VfxService : ILoadingModule, IDisposable
    {
        private readonly VfxFactory vfxFactory;
        private readonly TimeScaleHolder timeScaleHolder;

        private List<Vfx> playingEffects;


        public VfxService(
            VfxFactory vfxFactory,
            TimeScaleHolder timeScaleHolder)
        {
            this.vfxFactory = vfxFactory;
            this.timeScaleHolder = timeScaleHolder;

            playingEffects = new List<Vfx>();

        }

        public UniTask Load()
        {
            timeScaleHolder.GameplayVisualEffectsTimeScaleChanged += OnGameplayTimeScaleChanged;

            return UniTask.CompletedTask;
        }

        public void PlayEffect(VfxType type, Vector3 position, Color color)
        {
            Vfx vfx = vfxFactory.CreateVisualEffect(type);

            vfx.SetColor(color);

            PlayEffectInternal(vfx, position);
        }

        public void PlayEffect(VfxType type, Vector3 position, float radius)
        {
            Vfx vfx = vfxFactory.CreateVisualEffect(type);

            vfx.SetSize(radius);

            PlayEffectInternal(vfx, position);
        }

        public void PlayEffect(VfxType type, Vector3 position, Color color, float radius)
        {
            Vfx vfx = vfxFactory.CreateVisualEffect(type);

            vfx.SetColor(color);
            vfx.SetSize(radius * 2);

            PlayEffectInternal(vfx, position);
        }

        public void Dispose()
        {
            timeScaleHolder.GameplayVisualEffectsTimeScaleChanged -= OnGameplayTimeScaleChanged;
        }


        private void OnGameplayTimeScaleChanged()
        {
            foreach (Vfx playingEffect in playingEffects)
            {
                playingEffect.SetSimulationSpeed(timeScaleHolder.GameplayVisualEffectsTimeScale);
            }
        }

        private void PlayEffectInternal(Vfx vfx, Vector3 position)
        {
            playingEffects.Add(vfx);

            vfx.gameObject.transform.position = position;

            vfx.SetSimulationSpeed(timeScaleHolder.GameplayVisualEffectsTimeScale);
            vfx.Play();
            DestroyVisualEffectAfterPlay(vfx).Forget();
        }

        private async UniTask DestroyVisualEffectAfterPlay(Vfx vfx)
        {
            await UniTaskHelper.DelayWithGameplayVisualEffectsTimeScale(vfx.Duration, timeScaleHolder, vfx.destroyCancellationToken);

            playingEffects.Remove(vfx);
            vfxFactory.DestroyVisualEffect(vfx);
        }
    }
}