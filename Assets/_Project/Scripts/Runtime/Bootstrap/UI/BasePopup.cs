using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace ControllThemAll.Runtime.Bootstrap.UI
{
    public class BasePopup : MonoBehaviour
    {
        public Action Activated;
        public Action Deactivated;


        private MotionHandle showHideHandle;


        public bool Visible { get; private set; } = false;


        private void OnDestroy()
        {
            showHideHandle.TryComplete();
        }


        public virtual async UniTask Show()
        {
            Visible = true;

            Activated?.Invoke();
            gameObject.SetActive(true);
            showHideHandle.TryComplete();

            showHideHandle = LMotion.Create(Vector3.zero, transform.localScale, 0.2f)
                .WithEase(Ease.Linear)
                .BindToLocalScale(transform);

            await showHideHandle;
        }

        public virtual async UniTask Hide()
        {
            Visible = false;

            Deactivated?.Invoke();
            showHideHandle.TryComplete();

            Vector3 scale = transform.localScale;
            showHideHandle = LMotion.Create(scale, Vector3.zero, 0.2f)
                .WithEase(Ease.Linear)
                .BindToLocalScale(transform);

            await showHideHandle;

            gameObject.SetActive(false);

            transform.localScale = scale;
        }
    }
}