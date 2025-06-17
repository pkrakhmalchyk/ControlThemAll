using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Infrastructure
{
    public interface IDataHandler
    {
        UniTask Save<T>(string key, T data) where T : class;
        UniTask<T> Load<T>(string key) where T : class;
    }
}