using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.Bootstrap.UI
{
    public class BaseWindow : MonoBehaviour
    {
        public Action Activated;
        public Action Deactivated;


        public bool Visible { get; private set; } = false;


        public virtual UniTask Show()
        {
            Visible = true;

            Activated?.Invoke();
            gameObject.SetActive(true);

            return UniTask.CompletedTask;
        }

        public virtual UniTask Hide()
        {
            Visible = false;

            Deactivated?.Invoke();
            gameObject.SetActive(false);

            return UniTask.CompletedTask;
        }
    }
}