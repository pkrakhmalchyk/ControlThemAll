using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class CollisionInBoundsDetected : IAbilityCondition
    {
        private readonly CollisionInBoundsDetectedConfig config;
        private readonly IBoundsContext context;
        private CollisionDetectionService collisionDetectionService;


        public CollisionInBoundsDetected(
            CollisionInBoundsDetectedConfig config,
            IBoundsContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(CollisionDetectionService collisionDetectionService)
        {
            this.collisionDetectionService = collisionDetectionService;
        }


        public bool IsFulfilled()
        {
            return collisionDetectionService.IsCollisionInBoxDetected(
                context.Position,
                context.Bounds * config.BoundsMultiplier,
                context.Orientation,
                context.Owner.PhysicsLayer);
        }
    }
}