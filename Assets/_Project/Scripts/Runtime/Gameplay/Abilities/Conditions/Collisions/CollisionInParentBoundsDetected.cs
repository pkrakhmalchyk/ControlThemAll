using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class CollisionInParentBoundsDetected : IAbilityCondition
    {
        private readonly CollisionInParentBoundsDetectedConfig config;
        private readonly IParentBoundsContext context;
        private CollisionDetectionService collisionDetectionService;


        public CollisionInParentBoundsDetected(
            CollisionInParentBoundsDetectedConfig config,
            IParentBoundsContext context)
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