using ControllThemAll.Runtime.Gameplay;
using Newtonsoft.Json;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [CreateAssetMenu(menuName = "EnemyWeaponConfig/EnemyWeaponAbilityConfig")]
    public class EnemyWeaponAbilityConfig : JsonConvertableSO
    {
        public AbilityConfig<EnemyWeaponContext> Config;


        public override string ToJson()
        {
            return JsonConvert.SerializeObject(Config, Formatting.Indented,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        }
    }
}