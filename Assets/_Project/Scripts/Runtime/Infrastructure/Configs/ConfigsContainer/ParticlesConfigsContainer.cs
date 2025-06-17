using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Gameplay;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ControllThemAll.Runtime.Infrastructure
{
    public class ParticlesConfigsContainer : ILoadingModule, IDisposable
    {
        private Dictionary<string, ParticleConfig> particlesConfigs;
        private Dictionary<string, AbilityConfig<ParticleContext>> particlesAbilitiesConfigs;
        private AsyncOperationHandle<IList<ParticleConfig>> particlesConfigsHandle;


        public async UniTask Load()
        {
            await LoadConfigs();
        }

        public ParticleConfig GetParticleConfig(string particleId)
        {
            if (!particlesConfigs.TryGetValue(particleId, out ParticleConfig particleConfig))
            {
                Logging.Gameplay.LogError($"Particle config '{particleId}' was not found");

                throw new Exception($"Particle config '{particleId}' was not found");
            }

            return particleConfig;
        }

        public AbilityConfig<ParticleContext> GetParticleAbilityConfig(string id)
        {
            if (!particlesAbilitiesConfigs.TryGetValue(id, out AbilityConfig<ParticleContext> particleAbilityConfig))
            {
                Logging.Gameplay.LogError($"Particle ability config '{id}' was not found");

                throw new Exception($"Particle ability config '{id}' was not found");
            }

            return particleAbilityConfig;
        }

        public void Dispose()
        {
            Addressables.Release(particlesConfigsHandle);
        }


        private async UniTask LoadConfigs()
        {
            particlesConfigs = new Dictionary<string, ParticleConfig>();
            particlesConfigsHandle = Addressables.LoadAssetsAsync<ParticleConfig>(new []
            {
                $"{RuntimeConstants.Configs.ParticlesConfigsLabel}"
            }, null, Addressables.MergeMode.Union);
            particlesAbilitiesConfigs = new Dictionary<string, AbilityConfig<ParticleContext>>();
            AsyncOperationHandle<IList<TextAsset>> particlesAbilitiesConfigsHandle = Addressables.LoadAssetsAsync<TextAsset>(new []
            {
                $"{RuntimeConstants.Configs.ParticlesAbilitiesConfigsLabel}",
            }, null, Addressables.MergeMode.Union );

            IList<ParticleConfig> loadedParticlesConfigs = await particlesConfigsHandle;
            foreach (ParticleConfig particleConfig in loadedParticlesConfigs)
            {
                particlesConfigs.Add(particleConfig.name, particleConfig);
            }

            IList<TextAsset> particlesAbilitiesConfigsTexts = await particlesAbilitiesConfigsHandle;

            foreach (TextAsset particlesAbilityConfigText in particlesAbilitiesConfigsTexts)
            {
                AbilityConfig<ParticleContext> abilityConfig = JsonConvert.DeserializeObject<AbilityConfig<ParticleContext>>(particlesAbilityConfigText.text, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                particlesAbilitiesConfigs.Add(particlesAbilityConfigText.name, abilityConfig);
            }

            Addressables.Release(particlesAbilitiesConfigsHandle);
        }
    }
}