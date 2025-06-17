namespace ControllThemAll.Runtime.Gameplay
{
    public interface IGameplayEntity
    {
        public int Id { get; }
        public int PhysicsLayer { get; }
        public string GameplayLayer { get; }
        public IGameplayEntity Parent { get; }
    }
}