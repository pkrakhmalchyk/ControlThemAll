using ControllThemAll.Runtime.Gameplay;
using Newtonsoft.Json;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [CreateAssetMenu(menuName = "PlayerWeaponConfig/PlayerWeaponAbilityConfig")]
    public class PlayerWeaponAbilityConfig : JsonConvertableSO
    {
        public AbilityConfig<PlayerWeaponContext> Config;


        public override string ToJson()
        {
            return JsonConvert.SerializeObject(Config, Formatting.Indented,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}