using System.Collections;
using UnityEngine;

namespace ControllThemAll.Runtime.Bootstrap.UI
{
    public class RootUIView : MonoBehaviour
    {
        [SerializeField] private Transform windowContainer;
        [SerializeField] private Transform popupContainer;
        [SerializeField] private Transform loadingWindow;


        public void Initialize()
        {
            loadingWindow.gameObject.SetActive(false);
            popupContainer.gameObject.SetActive(false);
            windowContainer.gameObject.SetActive(true);
        }

        public void SetLoadingWindowActive(bool active)
        {
            loadingWindow.gameObject.SetActive(active);
        }

        public void SetWindowContainerActive(bool active)
        {
            windowContainer.gameObject.SetActive(active);
        }

        public void AddWindowToWindowsContainer(GameObject window)
        {
            window.transform.SetParent(windowContainer);
        }

        public void RemoveWindowFromWindowsContainer(GameObject window)
        {
            window.transform.SetParent(null);
        }

        public void SetPopupContainerActive(bool active)
        {
            popupContainer.gameObject.SetActive(active);
        }

        public void AddPopupToPopupsContainer(GameObject popup)
        {
            popup.transform.SetParent(popupContainer);
        }

        public void RemovePopupFromPopupsContainer(GameObject popup)
        {
            popup.transform.SetParent(null);
        }

        public bool HasVisiblePopups()
        {
            for (int i = 0; i < popupContainer.childCount; i++)
            {
                if (popupContainer.GetChild(i).gameObject.activeSelf)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
