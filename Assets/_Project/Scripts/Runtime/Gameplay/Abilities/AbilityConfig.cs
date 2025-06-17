using System;
using System.Collections.Generic;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [Serializable]
    public class AbilityConfig<T> where T : IAbilityContext
    {
        [SerializeReference, SubclassSelector]
        private List<IAbilityComponentConfig<T>> componentsConfigs = new List<IAbilityComponentConfig<T>>();
        [SerializeReference, SubclassSelector]
        private List<IAbilityConditionConfig<T>> conditionsConfigs = new List<IAbilityConditionConfig<T>>();


        public IEnumerable<IAbilityComponentConfig<T>> ComponentsConfigs => componentsConfigs;
        public IEnumerable<IAbilityConditionConfig<T>> ConditionsConfigs => conditionsConfigs;
    }
}