using System;
using System.Collections.Generic;
using System.Linq;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AbilitiesService : IDisposable
    {
        private readonly AbilityFactory abilityFactory;

        private Dictionary<int, List<Ability>> entitiesAbilities;


        public AbilitiesService(AbilityFactory abilityFactory)
        {
            this.abilityFactory = abilityFactory;

            entitiesAbilities = new Dictionary<int, List<Ability>>();
        }


        public void BindAbilities<T>(IEnumerable<AbilityConfig<T>> abilitiesConfigs, T context) where T : IAbilityContext
        {
            SetAbilitiesActive(context.Owner.Id, false);
            ClearAbilities(context.Owner.Id);

            entitiesAbilities.Add(context.Owner.Id, CreateAbilities(abilitiesConfigs, context));
        }

        public void ClearAbilities(int ownerId)
        {
            SetAbilitiesActive(ownerId, false);

            if (entitiesAbilities.TryGetValue(ownerId, out List<Ability> abilities))
            {
                foreach (Ability ability in abilities)
                {
                    ability.Dispose();
                }

                abilities.Clear();
                entitiesAbilities.Remove(ownerId);
            }
        }

        public void SetAbilitiesActive(int ownerId, bool active)
        {
            if (entitiesAbilities.TryGetValue(ownerId, out List<Ability> abilities))
            {
                foreach (Ability ability in abilities)
                {
                    ability.SetActive(active);
                }
            }
        }

        public void Dispose()
        {
            foreach (KeyValuePair<int, List<Ability>> entityAbilities in entitiesAbilities)
            {
                foreach (Ability ability in entityAbilities.Value)
                {
                    ability.Dispose();
                }
            }

            entitiesAbilities.Clear();
        }


        private List<Ability> CreateAbilities<T>(IEnumerable<AbilityConfig<T>> abilitiesConfigs, T context) where T : IAbilityContext
        {
            List<Ability> abilities = new List<Ability>(abilitiesConfigs.Count());

            foreach (AbilityConfig<T> abilityConfig in abilitiesConfigs)
            {
                abilities.Add(abilityFactory.CreateAbility(abilityConfig, context));
            }

            return abilities;
        }
    }
}