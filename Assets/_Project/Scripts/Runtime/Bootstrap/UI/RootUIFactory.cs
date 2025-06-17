using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ControllThemAll.Runtime.Bootstrap.UI
{
    public class RootUIFactory : ILoadingModule
    {
        private RootUIView rootUIPrefab;


        public UniTask Load()
        {
            LoadPrefabs();

            return UniTask.CompletedTask;
        }

        public RootUIAdapter CreateRootUI()
        {
            RootUIView rootUIView = Object.Instantiate(rootUIPrefab);
            RootUIAdapter rootUIAdapter = new RootUIAdapter(rootUIView);

            rootUIView.Initialize();
            Object.DontDestroyOnLoad(rootUIView);

            return rootUIAdapter;
        }


        private void LoadPrefabs()
        {
            rootUIPrefab = Resources.Load<RootUIView>($"{RuntimeConstants.UI.ResourcesRootUIPath}/{RuntimeConstants.UI.RootUIPrefabName}");
        }
    }
}
