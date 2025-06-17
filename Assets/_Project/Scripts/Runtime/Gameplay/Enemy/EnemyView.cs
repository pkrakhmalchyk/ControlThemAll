using ControllThemAll.Runtime.Infrastructure;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class EnemyView : MonoBehaviour, IGameplayEntity
    {
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private Transform weaponSlot;
        [SerializeField] private Animator animator;

        private int id;
        private string gameplayLayer;
        private IGameplayEntity parent;
        private Transform cachedTransform;
        private MaterialPropertyBlock materialPropertyBlock;

        private EnemyConfig config;
        private IMovementInput movementInput;
        private TimeScaleHolder timeScaleHolder;

        private Vector3 movementDirection;

        private readonly int fillRateId = Shader.PropertyToID("_Fill_Rate");
        private readonly int baseColorId = Shader.PropertyToID("_Base_Color");
        private readonly int fillColorId = Shader.PropertyToID("_Fill_Color");


        public int Id => id;
        public int PhysicsLayer => this == null ? 0 : gameObject.layer;
        public string GameplayLayer => gameplayLayer;
        public IGameplayEntity Parent => parent;
        public Transform WeaponSlot => weaponSlot;
        public float BoundsX => meshRenderer.bounds.extents.x;


        public void Initialize(
            int id,
            EnemyConfig config,
            IMovementInput movementInput,
            TimeScaleHolder timeScaleHolder)
        {
            cachedTransform = GetComponent<Transform>();
            materialPropertyBlock = new MaterialPropertyBlock();

            SetFillRate(1);

            this.id = id;
            this.config = config;
            this.movementInput = movementInput;
            this.timeScaleHolder = timeScaleHolder;
        }


        private void Update()
        {
            HandleInput();
            Move();

            animator.speed = timeScaleHolder.GameplayVisualEffectsTimeScale;
        }


        public void SetFillRate(float fillRate)
        {
            fillRate = Mathf.Clamp01(fillRate);

            materialPropertyBlock.SetFloat(fillRateId, fillRate);

            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetBaseColor(Color color)
        {
            materialPropertyBlock.SetColor(baseColorId, color);

            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetFillColor(Color color)
        {
            materialPropertyBlock.SetColor(fillColorId, color);

            meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetGameplayLayer(string gameplayLayer)
        {
            this.gameplayLayer = gameplayLayer;
        }

        public void SetParent(IGameplayEntity parent)
        {
            this.parent = parent;
        }

        public void PlayHitAnimation()
        {
            animator.SetTrigger(Animator.StringToHash("Hit"));
        }


        private void HandleInput()
        {
            movementDirection = movementInput.Direction;
        }

        private void Move()
        {
            Vector3 newPosition = cachedTransform.position + movementDirection * config.Speed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale;
            Quaternion targetRotation = movementDirection != Vector3.zero
                ? Quaternion.LookRotation(movementDirection)
                : Quaternion.identity;
            Quaternion newRotation
                = Quaternion.Slerp(cachedTransform.rotation, targetRotation, config.RotationSpeed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale);

            cachedTransform.position = newPosition;
            cachedTransform.rotation = newRotation;

            animator.SetBool(Animator.StringToHash("IsWalking"), movementDirection.magnitude * timeScaleHolder.GameplayTimeScale > 0);
        }
    }
}