namespace ControllThemAll.Runtime.Gameplay
{
    public interface IAbilityComponentConfig<in T> where T : IAbilityContext
    {
        public IAbilityComponent Create(T context);
    }
}