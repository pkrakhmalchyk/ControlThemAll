using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class StraightInputAim : IAbilityComponent, IDisposable
    {
        private readonly StraightInputAimConfig config;
        private readonly IInputAttackContext context;
        private AimRenderingService aimRenderingService;

        private CancellationTokenSource cts;
        private string aimTrajectoryRendererKey;
        private AimRenderingService.AimRenderingSettings aimRenderingSettings;


        public StraightInputAim(
            StraightInputAimConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;

            aimTrajectoryRendererKey = $"{this.context.Owner.Id}.{typeof(StraightInputAim)}TrajectoryRenderer";
            aimRenderingSettings = new AimRenderingService.AimRenderingSettings(0.2f, 0.05f);
        }

        [Inject]
        public void Initialize(AimRenderingService aimRenderingService)
        {
            this.aimRenderingService = aimRenderingService;
        }


        public void Execute(bool execute)
        {
            if (execute)
            {
                StartAim();
            }
            else
            {
                FinishAim();
            }
        }

        public void Dispose()
        {
            FinishAim();
        }


        private void StartAim()
        {
            cts = new CancellationTokenSource();

            UpdateAim(cts.Token).Forget();
        }

        private void FinishAim()
        {
            aimRenderingService.HideAim(aimTrajectoryRendererKey);
            aimRenderingService.RemoveAimRenderer(aimTrajectoryRendererKey);

            cts?.Cancel();
            cts?.Dispose();

            cts = null;
        }

        private async UniTask UpdateAim(CancellationToken cancellationToken)
        {
            aimRenderingService.AddAimRenderer(aimTrajectoryRendererKey, aimRenderingSettings);

            while (!cancellationToken.IsCancellationRequested)
            {
                Vector3 forceProjection = new Vector3(context.AttackInput.Force.x, 0, context.AttackInput.Force.z);
                List<Vector3> trajectoryPoints = TrajectoryPredictor.PredictStraightTrajectory(
                    context.ParticleSpawnPoint,
                    forceProjection,
                    config.MaxRange);

                aimRenderingService.RenderAim(aimTrajectoryRendererKey, trajectoryPoints);

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            }
        }
    }
}