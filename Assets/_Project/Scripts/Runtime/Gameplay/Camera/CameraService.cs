using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class CameraService : MonoBehaviour, ILoadingModule<IEnumerable<Transform>>, IDisposable
    {
        [SerializeField] private CinemachineBrain brain;
        [SerializeField] private CinemachineCamera activeGameplayCamera;
        [SerializeField] private CinemachineCamera startGameplayCamera;
        [SerializeField] private CinemachineTargetGroup cameraTargetGroup;

        private TimeScaleHolder timeScaleHolder;

        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
        private CancellationTokenSource shakeCts;


        public Vector3 CameraPosition => brain.OutputCamera.transform.position;
        public Vector3 CameraForward => brain.OutputCamera.transform.forward;
        public Vector3 CameraRight => brain.OutputCamera.transform.right;


        [Inject]
        public void Initialize(
            TimeScaleHolder timeScaleHolder)
        {
            this.timeScaleHolder = timeScaleHolder;
            cinemachineBasicMultiChannelPerlin = activeGameplayCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public UniTask Load(IEnumerable<Transform> cameraTargets)
        {
            cameraTargetGroup.Targets = new List<CinemachineTargetGroup.Target>(cameraTargets.Count());

            foreach (Transform target in cameraTargets)
            {
                cameraTargetGroup.AddMember(target, 1, 0);
            }

            if (cinemachineBasicMultiChannelPerlin != null)
            {
                cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0;
            }

            SetStartGameplayCameraActive();

            return UniTask.CompletedTask;
        }

        public Ray ScreenPointToRay(Vector3 position)
        {
            return brain.OutputCamera != null ? brain.OutputCamera.ScreenPointToRay(position) : new Ray();
        }

        public void Shake(float duration)
        {
            CancelActiveCameraShake();

            shakeCts = new CancellationTokenSource();

            ShakeActiveCamera(duration, shakeCts.Token).Forget();
        }

        public void SetStartGameplayCameraActive()
        {
            CancelActiveCameraShake();
            startGameplayCamera.gameObject.SetActive(true);
            activeGameplayCamera.gameObject.SetActive(false);
        }

        public void SetActiveGameplayCameraActive()
        {
            CancelActiveCameraShake();
            startGameplayCamera.gameObject.SetActive(false);
            activeGameplayCamera.gameObject.SetActive(true);
        }

        public void SetEndGameplayCameraActive()
        {
            CancelActiveCameraShake();
            startGameplayCamera.gameObject.SetActive(false);
            activeGameplayCamera.gameObject.SetActive(true);
        }

        public void Dispose()
        {
            CancelActiveCameraShake();
        }


        private async UniTask ShakeActiveCamera(float duration, CancellationToken token)
        {
            if (cinemachineBasicMultiChannelPerlin == null)
            {
                return;
            }

            cinemachineBasicMultiChannelPerlin.AmplitudeGain = 1;

            await UniTaskHelper.DelayWithGameplayVisualEffectsTimeScale(duration, timeScaleHolder, token);

            cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0;
        }

        private void CancelActiveCameraShake()
        {
            shakeCts?.Cancel();
            shakeCts?.Dispose();

            shakeCts = null;
        }
    }
}