using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Bootstrap.UI
{
    public class UIService : ILoadingModule
    {
        private readonly RootUIFactory rootUIFactory;

        private RootUIAdapter rootUIAdapter;

        private Dictionary<Type, BaseWindow> windows;
        private Dictionary<Type, BasePopup> popups;


        public UIService(RootUIFactory rootUIFactory)
        {
            this.rootUIFactory = rootUIFactory;
        }

        public UniTask Load()
        {
            rootUIAdapter = rootUIFactory.CreateRootUI();

            windows = new Dictionary<Type, BaseWindow>();
            popups = new Dictionary<Type, BasePopup>();

            return UniTask.CompletedTask;
        }

        public void SetLoadingWindowVisible(bool active)
        {
            rootUIAdapter.SetLoadingWindowVisible(active);
        }

        public void AddPopup(Type type, BasePopup popup)
        {
            popups.Add(type, popup);
            rootUIAdapter.AddPopup(popup);
        }

        public async UniTask SetPopupVisible(Type type, bool visible)
        {
            if (!popups.TryGetValue(type, out BasePopup popup))
            {
                Logging.UI.LogWarning($"Popup with type {type} does not exist");
                return;
            }

            if (popup.Visible == visible)
            {
                Logging.UI.LogWarning($"Popup with type {type} is already {(visible ? "visible" : "invisible")}");
                return;
            }

            await rootUIAdapter.SetPopupVisible(popup, visible);
        }

        public void RemovePopup(Type type)
        {
            if (!popups.Remove(type, out BasePopup popup))
            {
                Logging.UI.LogWarning($"Popup with type {type} does not exist");
                return;
            }

            rootUIAdapter.RemovePopup(popup);
        }

        public void AddWindow(Type type, BaseWindow window)
        {
            windows.Add(type, window);
            rootUIAdapter.AddWindow(window);
        }

        public async UniTask SetWindowVisible(Type type, bool visible)
        {
            if (!windows.TryGetValue(type, out BaseWindow window))
            {
                Logging.UI.LogWarning($"Window with type {type} does not exist");
                return;
            }

            if (window.Visible == visible)
            {
                Logging.UI.LogWarning($"Window with type {type} is already {(visible ? "visible" : "invisible")}");
                return;
            }

            List<UniTask> hideTasks = new List<UniTask>();

            foreach (BaseWindow existingWindow in windows.Values)
            {
                if (existingWindow.Visible)
                {
                    hideTasks.Add(rootUIAdapter.SetWindowVisible(existingWindow, false));
                }
            }

            await hideTasks;
            await rootUIAdapter.SetWindowVisible(window, visible);
        }

        public void RemoveWindow(Type type)
        {
            if (!windows.Remove(type, out BaseWindow window))
            {
                Logging.UI.LogWarning($"Window with type {type} does not exist");
                return;
            }

            rootUIAdapter.RemoveWindow(window);
        }
    }
}