using System.Collections.Generic;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AbilityFactory
    {
        private readonly IObjectResolver container;


        public AbilityFactory(IObjectResolver container)
        {
            this.container = container;
        }

        public Ability CreateAbility<T>(AbilityConfig<T> config, T context) where T : IAbilityContext
        {
            List<IAbilityComponent> components = new List<IAbilityComponent>();
            List<IAbilityCondition> conditions = new List<IAbilityCondition>();

            foreach (IAbilityComponentConfig<T> abilityComponentConfig in config.ComponentsConfigs)
            {
                components.Add(CreateAbilityComponent(abilityComponentConfig, context));
            }

            foreach (IAbilityConditionConfig<T> abilityConditionConfig in config.ConditionsConfigs)
            {
                conditions.Add(CreateAbilityCondition(abilityConditionConfig, context));
            }

            return new Ability(conditions, components);
        }


        private IAbilityComponent CreateAbilityComponent<T>(IAbilityComponentConfig<T> config, T context) where T : IAbilityContext
        {
            IAbilityComponent component = config.Create(context);

            container.Inject(component);

            return component;
        }

        private IAbilityCondition CreateAbilityCondition<T>(IAbilityConditionConfig<T> config, T context) where T : IAbilityContext
        {
            IAbilityCondition condition = config.Create(context);

            container.Inject(condition);

            return condition;
        }
    }
}