using System;
using ControllThemAll.Runtime.Shared;
using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    [Serializable]
    public class WeaponConfig
    {
        [ObjectNameSelector(typeof(GameObject))]
        public string PrefabName;
    }
}
