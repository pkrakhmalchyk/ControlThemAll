using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;


namespace ControllThemAll.Runtime.Infrastructure
{
    public interface ILoadingModule
    {
        UniTask Load();
    }

    public interface ILoadingModule<in T>
    {
        UniTask Load(T param);
    }

    public class ModulesLoader
    {
        public async UniTask Load(ILoadingModule loadingModule)
        {
            try
            {
                await loadingModule.Load();
            }
            catch (Exception e)
            {
                Logging.Loading.LogError(e);
            }
        }

        public async UniTask LoadParallel(IEnumerable<ILoadingModule> loadingModules)
        {
            try
            {
                UniTask task = UniTask.WhenAll(loadingModules.Select(loadingModule => loadingModule.Load()));

                await task;
            }
            catch (Exception e)
            {
                Logging.Loading.LogError(e);
            }
        }

        public async UniTask Load<T>(ILoadingModule<T> loadingModule, T param)
        {
            try
            {
                await loadingModule.Load(param);
            }
            catch (Exception e)
            {
                Logging.Loading.LogError(e);
            }
        }
    }
}
