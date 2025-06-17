using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace ControllThemAll.Runtime.Infrastructure
{
    public static class ScenesLoader
    {
        public static async UniTask LoadScene(int sceneIndexToLoad)
        {
            bool loadEmptyScene = sceneIndexToLoad == RuntimeConstants.Scenes.MainMenu || sceneIndexToLoad == RuntimeConstants.Scenes.Gameplay;

            if (loadEmptyScene)
            {
                await SceneManager.LoadSceneAsync(RuntimeConstants.Scenes.Empty).ToUniTask();
            }

            await SceneManager.LoadSceneAsync(sceneIndexToLoad).ToUniTask();
        }
    }
}
