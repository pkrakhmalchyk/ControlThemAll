using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class ParticleFactory : ILoadingModule, IDisposable
    {
        private readonly IObjectResolver container;

        private Dictionary<string, ObjectPool<ParticleView>> particlesPools;
        private Dictionary<string, ParticleView> particlesPrefabs;
        private List<AsyncOperationHandle<GameObject>> particlesPrefabsHandles;


        public ParticleFactory(IObjectResolver container)
        {
            this.container = container;
        }

        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public ParticleView CreateParticle(ParticleConfig config, bool isPlayer)
        {
            if (!particlesPools.TryGetValue(config.PrefabName, out ObjectPool<ParticleView> pool))
            {
                Logging.Gameplay.LogError($"Particle prefab with name {config.PrefabName} was not found");

                throw new Exception($"Particle prefab with name {config.PrefabName} was not found");
            }

            ParticleView particle = pool.Get();

            particle.SetParticleConfig(config);
            particle.gameObject.layer = isPlayer
                ? LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.PlayerParticle)
                : LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnemyParticle);
            particle.gameObject.SetActive(false);

            return particle;
        }

        public void DestroyParticle(ParticleView particle)
        {
            particle.gameObject.SetActive(false);

            if (particlesPools.TryGetValue(particle.GetParticleConfig().PrefabName, out ObjectPool<ParticleView> pool))
            {
                pool.Release(particle);
            }
        }

        public void Dispose()
        {
            foreach (ObjectPool<ParticleView> particlesPool in particlesPools.Values)
            {
                particlesPool.Dispose();
            }

            foreach (AsyncOperationHandle<GameObject> particlePrefabHandle in particlesPrefabsHandles)
            {
                Addressables.Release(particlePrefabHandle);
            }
        }


        private async UniTask LoadPrefabs()
        {
            particlesPrefabs = new Dictionary<string, ParticleView>();
            particlesPools = new Dictionary<string, ObjectPool<ParticleView>>();
            particlesPrefabsHandles = new List<AsyncOperationHandle<GameObject>>();

            foreach (string prefabName in RuntimeConstants.Particles.All)
            {
                particlesPrefabsHandles.Add(Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.Particles.ParticlesPath}/{prefabName}"));
            }

            IEnumerable<GameObject> loadedParticlesGameObjects = await UniTask.WhenAll(particlesPrefabsHandles.Select(prefabHandle => prefabHandle.ToUniTask()));
            IEnumerable<ParticleView> loadedParticlesPrefabs = loadedParticlesGameObjects.Select(gameObject => gameObject.GetComponent<ParticleView>());

            foreach (ParticleView loadedParticlePrefab in loadedParticlesPrefabs)
            {
                particlesPrefabs.Add(loadedParticlePrefab.name, loadedParticlePrefab);
                particlesPools.Add(loadedParticlePrefab.name, new ObjectPool<ParticleView>(
                    () =>
                    {
                        ParticleView particle = UnityEngine.Object.Instantiate(particlesPrefabs[loadedParticlePrefab.name]);

                        particle.Initialize(
                            container.Resolve<IdGenerator>().GetNextId(),
                            container.Resolve<TimeScaleHolder>());

                        return particle;
                    },
                    null,
                    null,
                    (particle) =>
                    {
                        if (particle != null)
                        {
                            UnityEngine.Object.Destroy(particle.gameObject);
                        }
                    }));
            }
        }
    }
}