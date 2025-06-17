using System;
using System.Collections.Generic;
using System.Threading;
using ControllThemAll.Runtime.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ControllThemAll.Runtime.Gameplay
{
    public class SwipeHorizontalMovementInput : IHorizontalMovementInput, ILoadingModule, IDisposable
    {
        private readonly ITouchInput touchInput;

        private CancellationTokenSource cts;
        private Vector2 startTouchPosition;
        private bool isSwipeActive = false;
        private float minSwipeDistance = 50f;


        public float Input { get; private set; }


        public SwipeHorizontalMovementInput(ITouchInput touchInput)
        {
            this.touchInput = touchInput;
        }

        public UniTask Load()
        {
            cts = new CancellationTokenSource();

            CheckSwipeEvents(cts.Token).Forget();

            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();

            cts = null;
        }


        private async UniTask CheckSwipeEvents(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!isSwipeActive && touchInput.IsActive)
                {
                    if (!IsPointerOverUI())
                    {
                        startTouchPosition = touchInput.Position;
                        isSwipeActive = true;
                    }
                    else
                    {
                        while (touchInput.IsActive)
                        {
                            await UniTask.Yield(cancellationToken);
                        }
                    }
                }

                if (isSwipeActive)
                {
                    float distanceX = touchInput.Position.x - startTouchPosition.x;
                    float distanceY = touchInput.Position.y - startTouchPosition.y;

                    if (Mathf.Abs(distanceX) > Mathf.Abs(distanceY) && Mathf.Abs(distanceX) > minSwipeDistance)
                    {
                        Input = distanceX > 0 ? 1 : -1;
                    }
                }

                if (isSwipeActive && !touchInput.IsActive)
                {
                    Input = 0;
                    isSwipeActive = false;
                }

                await UniTask.Yield(cancellationToken);
            }
        }

        private bool IsPointerOverUI()
        {
            EventSystem eventSystem = EventSystem.current;
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = touchInput.Position
            };
            List<RaycastResult> results = new List<RaycastResult>();

            eventSystem.RaycastAll(pointerData, results);

            return results.Count > 0;
        }
    }
}