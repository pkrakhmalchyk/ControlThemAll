using ControllThemAll.Runtime.Gameplay;
using Newtonsoft.Json;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [CreateAssetMenu(menuName = "ParticleConfig/ParticleAbilityConfig")]
    public class ParticleAbilityConfig : JsonConvertableSO
    {
        public AbilityConfig<ParticleContext> Config;

        public override string ToJson()
        {
            return JsonConvert.SerializeObject(Config, Formatting.Indented,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}