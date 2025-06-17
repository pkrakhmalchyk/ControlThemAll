using System;
using System.Collections.Generic;
using System.Linq;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace ControllThemAll.Runtime.Gameplay
{
    public class VfxFactory : ILoadingModule, IDisposable
    {
        private Dictionary<string, Vfx> visualEffectsPrefabs;
        private List<AsyncOperationHandle<GameObject>> visualEffectsPrefabsHandles;


        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public Vfx CreateVisualEffect(VfxType type)
        {
            Vfx vfxPrefab = GetVisualEffectPrefabByType(type);
            Vfx vfx = Object.Instantiate(vfxPrefab);

            vfx.Initialize();
            vfx.gameObject.SetActive(false);

            return vfx;
        }

        public void DestroyVisualEffect(Vfx vfx)
        {
            if (vfx != null)
            {
                Object.Destroy(vfx.gameObject);
            }
        }

        public void Dispose()
        {
            foreach (AsyncOperationHandle<GameObject> visualEffectPrefabHandle in visualEffectsPrefabsHandles)
            {
                Addressables.Release(visualEffectPrefabHandle);
            }
        }


        private Vfx GetVisualEffectPrefabByType(VfxType type)
        {
            foreach (Vfx vfx in visualEffectsPrefabs.Values)
            {
                if (vfx.Type == type)
                {
                    return vfx;
                }
            }

            Logging.Gameplay.LogError($"Visual effect prefab with type {type} was not found");

            throw new Exception($"Weapon prefab with type {type} was not found");
        }

        private async UniTask LoadPrefabs()
        {
            visualEffectsPrefabs = new Dictionary<string, Vfx>();
            visualEffectsPrefabsHandles = new List<AsyncOperationHandle<GameObject>>();

            foreach (string prefabName in RuntimeConstants.VisualEffects.All)
            {
                visualEffectsPrefabsHandles.Add(Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.VisualEffects.VisualEffectsPath}/{prefabName}"));
            }

            IEnumerable<GameObject> loadedVisualEffectsGameObjects = await UniTask.WhenAll(visualEffectsPrefabsHandles.Select(prefabHandle => prefabHandle.ToUniTask()));
            IEnumerable<Vfx> loadedVisualEffectsPrefabs = loadedVisualEffectsGameObjects.Select(gameObject => gameObject.GetComponent<Vfx>());

            foreach (Vfx visualEffectPrefab in loadedVisualEffectsPrefabs)
            {
                visualEffectsPrefabs.Add(visualEffectPrefab.name, visualEffectPrefab);
            }
        }
    }
}