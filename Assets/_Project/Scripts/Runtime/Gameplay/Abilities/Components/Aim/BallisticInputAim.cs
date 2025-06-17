using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ControllThemAll.Runtime.Utils;
using UnityEngine;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class BallisticInputAim : IAbilityComponent, IDisposable
    {
        private readonly BallisticInputAimConfig config;
        private readonly IInputAttackContext context;
        private AimRenderingService aimRenderingService;

        private CancellationTokenSource cts;
        private float aimAreaRaycastDistance = 10f;
        private int aimAreaPointsCount = 20;
        private string aimTrajectoryRendererKey;
        private string aimAreaRendererKey;
        private AimRenderingService.AimRenderingSettings aimRenderingSettings;


        public BallisticInputAim(
            BallisticInputAimConfig config,
            IInputAttackContext context)
        {
            this.config = config;
            this.context = context;

            aimTrajectoryRendererKey = $"{this.context.Owner.Id}.{typeof(BallisticInputAim)}TrajectoryRenderer";
            aimAreaRendererKey = $"{this.context.Owner.Id}.{typeof(BallisticInputAim)}AreaRenderer";
            aimRenderingSettings = new AimRenderingService.AimRenderingSettings(0.2f, 0.2f);
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
            aimRenderingService.HideAim(aimAreaRendererKey);
            aimRenderingService.RemoveAimRenderer(aimTrajectoryRendererKey);
            aimRenderingService.RemoveAimRenderer(aimAreaRendererKey);

            cts?.Cancel();
            cts?.Dispose();

            cts = null;
        }

        private async UniTask UpdateAim(CancellationToken cancellationToken)
        {
            aimRenderingService.AddAimRenderer(aimTrajectoryRendererKey, aimRenderingSettings);
            aimRenderingService.AddAimRenderer(aimAreaRendererKey, aimRenderingSettings);

            while (!cancellationToken.IsCancellationRequested)
            {
                float attackForceMagnitude =
                    Mathf.Clamp(context.AttackInput.Force.magnitude, config.MinAttackForce, config.MaxAttackForce);
                List<Vector3> trajectoryPoints = TrajectoryPredictor.PredictBallisticTrajectory(
                    context.ParticleSpawnPoint,
                    context.AttackInput.Force.normalized * attackForceMagnitude,
                    config.MaxRange);

                aimRenderingService.RenderAim(aimTrajectoryRendererKey, trajectoryPoints);

                if (trajectoryPoints.Count > 0)
                {
                    List<Vector3> aimAreaPoints = CalculateAimAreaPoints(trajectoryPoints.Last());

                    aimRenderingService.RenderAim(aimAreaRendererKey, aimAreaPoints);
                }

                await UniTask.Yield(PlayerLoopTiming.FixedUpdate, cancellationToken);
            }
        }

        private List<Vector3> CalculateAimAreaPoints(Vector3 point)
        {
            Vector3 aimAreaCenterPoint = Physics.Raycast(point, Vector3.down, out RaycastHit hitInfo, aimAreaRaycastDistance,
                LayerMask.GetMask(RuntimeConstants.PhysicLayers.Brick))
                ? hitInfo.point
                : point;

            List<Vector3> aimAreaPoints = new List<Vector3>();

            for (int i = 0; i < aimAreaPointsCount + 1; i++)
            {
                float angle = i * 2f * Mathf.PI / aimAreaPointsCount;
                float x = Mathf.Sin(angle);
                float z = Mathf.Cos(angle);

                aimAreaPoints.Add(aimAreaCenterPoint + new Vector3(x, 0, z) * config.AimAreaRadius);
            }

            return aimAreaPoints;
        }
    }
}