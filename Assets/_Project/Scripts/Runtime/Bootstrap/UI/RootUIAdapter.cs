using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Bootstrap.UI
{
    public class RootUIAdapter
    {
        private readonly RootUIView rootUIView;

        public RootUIAdapter(RootUIView rootUIView)
        {
            this.rootUIView = rootUIView;
        }

        public void SetLoadingWindowVisible(bool active)
        {
            rootUIView.SetLoadingWindowActive(active);
        }

        public void AddPopup(BasePopup popup)
        {
            popup.gameObject.SetActive(false);
            rootUIView.AddPopupToPopupsContainer(popup.gameObject);
        }

        public async UniTask SetPopupVisible(BasePopup popup, bool visible)
        {
            if (visible)
            {
                rootUIView.SetPopupContainerActive(true);

                await popup.Show();
            }
            else
            {
                await popup.Hide();

                rootUIView.SetPopupContainerActive(rootUIView.HasVisiblePopups());
            }
        }

        public void RemovePopup(BasePopup popup)
        {
            if (popup == null)
            {
                return;
            }

            popup.Hide().Forget();
            popup.gameObject.SetActive(false);
            rootUIView.RemovePopupFromPopupsContainer(popup.gameObject);
            rootUIView.SetPopupContainerActive(rootUIView.HasVisiblePopups());
        }

        public void AddWindow(BaseWindow window)
        {
            window.gameObject.SetActive(false);
            rootUIView.AddWindowToWindowsContainer(window.gameObject);
        }

        public async UniTask SetWindowVisible(BaseWindow window, bool visible)
        {
            if (visible)
            {
                await window.Show();
            }
            else
            {
                await window.Hide();
            }
        }

        public void RemoveWindow(BaseWindow window)
        {
            if (window == null)
            {
                return;
            }

            window.Hide().Forget();
            window.gameObject.SetActive(false);
            rootUIView.RemoveWindowFromWindowsContainer(window.gameObject);
        }
    }
}
