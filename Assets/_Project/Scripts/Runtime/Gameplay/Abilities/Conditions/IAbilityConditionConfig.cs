namespace ControllThemAll.Runtime.Gameplay
{
    public interface IAbilityConditionConfig<in T> where T : IAbilityContext
    {
        public IAbilityCondition Create(T context);
    }
}