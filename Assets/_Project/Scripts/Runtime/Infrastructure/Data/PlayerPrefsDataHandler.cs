using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    public class PlayerPrefsDataHandler : IDataHandler
    {
        public UniTask Save<T>(string key, T data) where T : class
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();

            return UniTask.CompletedTask;
        }

        public UniTask<T> Load<T>(string key) where T : class
        {
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                T data = JsonConvert.DeserializeObject<T>(json);

                return new UniTask<T>(data);
            }

            return new UniTask<T>(null);
        }
    }
}