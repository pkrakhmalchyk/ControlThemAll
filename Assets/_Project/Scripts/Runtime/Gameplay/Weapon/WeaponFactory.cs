using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class WeaponFactory : ILoadingModule, IDisposable
    {
        private readonly IObjectResolver container;

        private Dictionary<string, WeaponView> weaponsPrefabs;
        private List<AsyncOperationHandle<GameObject>> weaponsPrefabsHandles;


        public WeaponFactory(IObjectResolver container)
        {
            this.container = container;
        }

        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public WeaponView CreateWeapon(WeaponConfig config, bool isPlayer)
        {
            if (!weaponsPrefabs.TryGetValue(config.PrefabName, out WeaponView weaponPrefab))
            {
                Logging.Gameplay.LogError($"Weapon prefab with name {config.PrefabName} was not found");

                throw new Exception($"Weapon prefab with name {config.PrefabName} was not found");
            }

            WeaponView weapon = UnityEngine.Object.Instantiate(weaponPrefab);
            weapon.gameObject.layer = isPlayer
                ? LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.PlayerWeapon)
                : LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnemyWeapon);
            weapon.gameObject.SetActive(false);

            weapon.Initialize(
                container.Resolve<IdGenerator>().GetNextId(),
                container.Resolve<TimeScaleHolder>());

            return weapon;
        }

        public void Dispose()
        {
            foreach (AsyncOperationHandle<GameObject> weaponPrefabHandle in weaponsPrefabsHandles)
            {
                Addressables.Release(weaponPrefabHandle);
            }
        }


        private async UniTask LoadPrefabs()
        {
            weaponsPrefabs = new Dictionary<string, WeaponView>();
            weaponsPrefabsHandles = new List<AsyncOperationHandle<GameObject>>();

            foreach (string prefabName in RuntimeConstants.Weapons.All)
            {
                weaponsPrefabsHandles.Add(Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.Weapons.WeaponsPath}/{prefabName}"));
            }

            IEnumerable<GameObject> loadedWeaponsGameObjects = await UniTask.WhenAll(weaponsPrefabsHandles.Select(prefabHandle => prefabHandle.ToUniTask()));
            IEnumerable<WeaponView> loadedWeaponsPrefabs = loadedWeaponsGameObjects.Select(gameObject => gameObject.GetComponent<WeaponView>());


            foreach (WeaponView weaponPrefab in loadedWeaponsPrefabs)
            {
                weaponsPrefabs.Add(weaponPrefab.name, weaponPrefab);
            }
        }
    }
}