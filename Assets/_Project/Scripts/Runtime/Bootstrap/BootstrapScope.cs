using ControllThemAll.Runtime.Bootstrap.UI;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Shared;
using ControllThemAll.Runtime.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace ControllThemAll.Runtime.Bootstrap
{
    public class BootstrapScope : LifetimeScope
    {
        private static int nextSceneIndex = -1;

        [SerializeField] private RootUIView rootUIPrefab;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static async void Initialize()
        {
            nextSceneIndex = SceneManager.GetActiveScene().buildIndex;

            if (nextSceneIndex == RuntimeConstants.Scenes.MainMenu || nextSceneIndex == RuntimeConstants.Scenes.Gameplay)
            {
                await ScenesLoader.LoadScene(RuntimeConstants.Scenes.Empty);
                await ScenesLoader.LoadScene(RuntimeConstants.Scenes.Bootstrap);
            }
        }

        protected override void Awake()
        {
            DontDestroyOnLoad(this);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ModulesLoader>(Lifetime.Scoped);
            builder.Register<IDataHandler, PlayerPrefsDataHandler>(Lifetime.Scoped);
            builder.Register<LevelsDataService>(Lifetime.Singleton);
            builder.Register<RootUIFactory>(Lifetime.Singleton);
            builder.Register<UIService>(Lifetime.Singleton);

            builder.RegisterEntryPoint<BootstrapEntryPoint>().WithParameter(nextSceneIndex);
        }
    }
}
