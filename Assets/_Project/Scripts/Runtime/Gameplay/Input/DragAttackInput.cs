using System;
using System.Threading;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class DragAttackInput : IAttackInput, ILoadingModule, IDisposable
    {
        private readonly CameraService cameraService;
        private readonly ITouchInput touchInput;
        private readonly LayerMask layerMask;

        private Plane plane;
        private float sensitivity = 1f;
        private float maxForceHorizontalAngle = 180;
        private bool isAttackActive = false;
        private Vector3 startInputPosition;
        private Vector3 attackForce;
        private CancellationTokenSource cts;


        public bool IsActive => isAttackActive;
        public Vector3 Force => attackForce;


        public DragAttackInput(CameraService cameraService, ITouchInput touchInput, LayerMask layerMask)
        {
            this.cameraService = cameraService;
            this.touchInput = touchInput;
            this.layerMask = layerMask;

        }

        public UniTask Load()
        {
            cts = new CancellationTokenSource();

            CheckDragEvents(cts.Token).Forget();

            return UniTask.CompletedTask;
        }

        public void SetSensitivity(float sensitivity)
        {
            this.sensitivity = sensitivity;
        }

        public void SetMaxForceHorizontalAngle(float maxForceHorizontalAngle)
        {
            this.maxForceHorizontalAngle = maxForceHorizontalAngle;
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();

            cts = null;
        }


        private async UniTask CheckDragEvents(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!isAttackActive && touchInput.IsActive)
                {
                    if (IsOnTriggerLayer())
                    {
                        isAttackActive = true;
                        startInputPosition = touchInput.Position;
                    }
                    else
                    {
                        while (touchInput.IsActive)
                        {
                            await UniTask.Yield(cancellationToken);
                        }
                    }
                }

                if (isAttackActive)
                {
                    attackForce = GetAttackForce(startInputPosition, touchInput.Position);
                }

                if (isAttackActive && !touchInput.IsActive)
                {
                    isAttackActive = false;
                }

                await UniTask.Yield(cancellationToken);
            }
        }

        private bool IsOnTriggerLayer()
        {
            Ray ray = cameraService.ScreenPointToRay(touchInput.Position);

            if (Physics.SphereCast(ray, RuntimeConstants.Settings.DragAttackInputSphereCastRadius, Mathf.Infinity, layerMask))
            {
                return true;
            }

            return false;
        }

        private Vector3 GetAttackForce(Vector3 startInputPosition, Vector3 currentInputPosition)
        {
            Vector3 startWorldPressPosition =
                GetWorldPointPositionWithAngleToPlaneNormal(startInputPosition, Vector3.up, Vector3.right, RuntimeConstants.Settings.DragAttackInputPlaneAngle);
            Vector3 currentWorldPressPosition =
                GetWorldPointPositionWithAngleToPlaneNormal(currentInputPosition, Vector3.up, Vector3.right, RuntimeConstants.Settings.DragAttackInputPlaneAngle);
            Vector3 force = startWorldPressPosition - currentWorldPressPosition;
            Vector3 projectedForce = Vector3.ProjectOnPlane(force, Vector3.up);
            float horizontalAngle = Vector3.SignedAngle(projectedForce, Vector3.forward, Vector3.up);

            if (Mathf.Abs(horizontalAngle) <= maxForceHorizontalAngle)
            {
                return force;
            }

            float clampedHorizontalAngle = Mathf.Clamp(-horizontalAngle, -maxForceHorizontalAngle, maxForceHorizontalAngle);

            return Quaternion.AngleAxis(clampedHorizontalAngle, Vector3.up)
                * Vector3.ProjectOnPlane(force.normalized, Vector3.right)
                * force.magnitude;
        }

        private Vector3 GetWorldPointPositionWithAngleToPlaneNormal(Vector3 screenPosition, Vector3 planeNormal, Vector3 angleAxis, float planeAngle)
        {
            Vector3 planeNormalWithAngle = Quaternion.AngleAxis(planeAngle, angleAxis) * planeNormal;

            return GetWorldPointPosition(screenPosition, planeNormalWithAngle);
        }

        private Vector3 GetWorldPointPosition(Vector3 screenPosition, Vector3 planeNormal)
        {
            Ray ray = cameraService.ScreenPointToRay(screenPosition);
            Vector3 planePosition = cameraService.CameraPosition
                + cameraService.CameraForward * RuntimeConstants.Settings.DragAttackInputPlaneDistance * sensitivity;

            plane.SetNormalAndPosition(planeNormal, planePosition);

            if (plane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
}