using Cysharp.Threading.Tasks;
using ControllThemAll.Runtime.Infrastructure;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class PlayerView : MonoBehaviour, IGameplayEntity
    {
        [SerializeField] private Transform weaponSlot;
        [SerializeField] private Transform particleSpawnPoint;
        [SerializeField] private SkinnedMeshRenderer playerMeshRenderer;
        [SerializeField] private Animator animator;
        [Header("Attack Indicator")]
        [SerializeField] private MeshRenderer attackIndicatorMeshRenderer;
        [SerializeField] private float startAttackIndicatorScale = 0.2f;
        [SerializeField] private float endAttackIndicatorScale = 0.4f;
        [SerializeField] private float attackIndicatorShowAnimationDuration = 0.2f;
        [SerializeField] private Ease attackIndicatorShowAnimationEase = Ease.Linear;
        [SerializeField] private float attackIndicatorHideAnimationDuration = 0.2f;
        [SerializeField] private Ease attackIndicatorHideAnimationEase = Ease.Linear;


        private int id;
        private string gameplayLayer;
        private IGameplayEntity parent;
        private Transform cachedTransform;

        private MaterialPropertyBlock playerMaterialPropertyBlock;
        private IMovementInput movementInput;
        private PlayerConfig playerConfig;
        private TimeScaleHolder timeScaleHolder;
        private MotionHandle attackIndicatorShowHideHandle;

        private bool isBeingPushed = false;
        private bool isJumping = false;
        private Vector3 movementDirection;

        private readonly int fillRateId = Shader.PropertyToID("_Fill_Rate");
        private readonly int baseColorId = Shader.PropertyToID("_Base_Color");
        private readonly int fillColorId = Shader.PropertyToID("_Fill_Color");


        public int Id => id;
        public int PhysicsLayer => this == null ? 0 : gameObject.layer;
        public string GameplayLayer => gameplayLayer;
        public IGameplayEntity Parent => parent;
        public Transform WeaponSlot => weaponSlot;
        public Transform ParticleSpawnPoint => particleSpawnPoint;
        public float BoundsX => playerMeshRenderer.bounds.extents.x;


        public void Initialize(
            int id,
            PlayerConfig playerConfig,
            IMovementInput movementInput,
            TimeScaleHolder timeScaleHolder)
        {
            cachedTransform = GetComponent<Transform>();

            playerMaterialPropertyBlock = new MaterialPropertyBlock();

            SetFillRate(1);
            attackIndicatorMeshRenderer.gameObject.SetActive(false);

            this.id = id;
            this.playerConfig = playerConfig;
            this.movementInput = movementInput;
            this.timeScaleHolder = timeScaleHolder;
        }


        private void Update()
        {
            if (isBeingPushed || isJumping)
            {
                return;
            }

            HandleInput();
            Move();

            animator.speed = timeScaleHolder.GameplayVisualEffectsTimeScale;
        }

        private void OnDestroy()
        {
            attackIndicatorShowHideHandle.TryCancel();
        }


        public void Push(Vector3 targetPosition, float duration)
        {
            if (isBeingPushed || isJumping)
            {
                return;
            }

            PushInternal(targetPosition, duration).Forget();
        }

        public void Jump(Vector3 targetPosition, float duration)
        {
            if (isBeingPushed || isJumping)
            {
                return;
            }

            JumpInternal(targetPosition, duration).Forget();
        }

        public void SetFillRate(float fillRate)
        {
            fillRate = Mathf.Clamp01(fillRate);

            playerMaterialPropertyBlock.SetFloat(fillRateId, fillRate);

            playerMeshRenderer.SetPropertyBlock(playerMaterialPropertyBlock);
        }

        public void SetBaseColor(Color color)
        {
            playerMaterialPropertyBlock.SetColor(baseColorId, color);

            playerMeshRenderer.SetPropertyBlock(playerMaterialPropertyBlock);
        }

        public void SetFillColor(Color color)
        {
            playerMaterialPropertyBlock.SetColor(fillColorId, color);
            playerMeshRenderer.SetPropertyBlock(playerMaterialPropertyBlock);
        }

        public void SetAttackIndicatorScale(float scale)
        {
            float scalesDifference = endAttackIndicatorScale - startAttackIndicatorScale;
            scale = startAttackIndicatorScale + Mathf.Clamp01(scale) * scalesDifference;
            Vector3 newScale = new Vector3(scale, 1, scale);
            attackIndicatorMeshRenderer.transform.localScale = newScale;
        }

        public async UniTask PlayShowAttackIndicatorAnimation()
        {
            attackIndicatorShowHideHandle.TryComplete();
            attackIndicatorMeshRenderer.gameObject.SetActive(true);

            attackIndicatorShowHideHandle = LMotion.Create(Vector3.zero, new Vector3(startAttackIndicatorScale, 1, startAttackIndicatorScale), attackIndicatorShowAnimationDuration)
                .WithEase(attackIndicatorShowAnimationEase)
                .BindToLocalScale(attackIndicatorMeshRenderer.transform);

            await attackIndicatorShowHideHandle;
        }

        public async UniTask PlayHideAttackIndicatorAnimation()
        {
            attackIndicatorShowHideHandle.TryComplete();

            attackIndicatorShowHideHandle = LMotion.Create(new Vector3(startAttackIndicatorScale, 1, startAttackIndicatorScale), Vector3.zero, attackIndicatorHideAnimationDuration)
                .WithEase(attackIndicatorHideAnimationEase)
                .BindToLocalScale(attackIndicatorMeshRenderer.transform);

            await attackIndicatorShowHideHandle;

            attackIndicatorMeshRenderer.gameObject.SetActive(false);
        }

        public void SetGameplayLayer(string gameplayLayer)
        {
            this.gameplayLayer = gameplayLayer;
        }

        public void SetParent(IGameplayEntity parent)
        {
            this.parent = parent;
        }

        public void PlayShootAnimation()
        {
            animator.SetTrigger(Animator.StringToHash("Shoot"));
        }

        public void PlayHitAnimation()
        {
            animator.SetTrigger(Animator.StringToHash("Hit"));
        }


        private void HandleInput()
        {
            movementDirection = movementInput?.Direction ?? Vector3.zero;
        }

        private void Move()
        {
            Vector3 newPosition = cachedTransform.position + movementDirection * playerConfig.Speed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale;
            Quaternion targetRotation = movementDirection != Vector3.zero
                ? Quaternion.LookRotation(movementDirection)
                : Quaternion.identity;
            Quaternion newRotation
                = Quaternion.Slerp(cachedTransform.rotation, targetRotation, playerConfig.RotationSpeed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale);

            cachedTransform.position = newPosition;
            cachedTransform.rotation = newRotation;

            animator.SetBool(Animator.StringToHash("IsWalking"), movementDirection.magnitude * timeScaleHolder.GameplayTimeScale > 0);
        }

        private async UniTaskVoid PushInternal(Vector3 targetPosition, float duration)
        {
            isBeingPushed = true;

            Vector3 direction = (targetPosition - cachedTransform.position).normalized;
            Quaternion targetRotation = direction != Vector3.zero ? Quaternion.LookRotation(direction) : Quaternion.identity;
            float pushLenght = (targetPosition - cachedTransform.position).magnitude;
            float passedTime = 0;
            float speed = pushLenght / duration;

            while (passedTime < duration)
            {
                Vector3 moveStep = direction * speed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale;
                Quaternion newRotation
                    = Quaternion.Slerp(cachedTransform.rotation, targetRotation, playerConfig.RotationSpeed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale);

                if (moveStep.magnitude >= Vector3.Distance(cachedTransform.position, targetPosition))
                {
                    cachedTransform.position = targetPosition;
                }
                else
                {
                    cachedTransform.position += moveStep;
                }

                cachedTransform.rotation = newRotation;

                animator.SetBool(Animator.StringToHash("IsWalking"), direction.magnitude * timeScaleHolder.GameplayTimeScale > 0);

                passedTime += Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale;

                await UniTask.Yield(destroyCancellationToken);
            }

            cachedTransform.position = targetPosition;
            isBeingPushed = false;
        }

        private async UniTaskVoid JumpInternal(Vector3 targetPosition, float duration)
        {
            isJumping = true;

            bool isLanding = false;

            Vector3 direction = (targetPosition - cachedTransform.position).normalized;
            float jumpLenght = (targetPosition - cachedTransform.position).magnitude;
            Quaternion targetRotation = direction != Vector3.zero ? Quaternion.LookRotation(direction) : Quaternion.identity;
            float passedTime = 0;
            float speed = jumpLenght / duration;

            animator.SetTrigger(Animator.StringToHash("Jump"));

            while (passedTime < duration)
            {
                Vector3 moveStep = direction * speed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale;
                Quaternion newRotation
                    = Quaternion.Slerp(cachedTransform.rotation, targetRotation, playerConfig.RotationSpeed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale);

                if (moveStep.magnitude >= Vector3.Distance(cachedTransform.position, targetPosition))
                {
                    cachedTransform.position = targetPosition;
                }
                else
                {
                    cachedTransform.position += moveStep;
                }

                cachedTransform.rotation = newRotation;

                if (!isLanding && passedTime / duration > 0.5f)
                {
                    animator.SetTrigger(Animator.StringToHash("Land"));

                    isLanding = true;
                }

                passedTime += Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale;

                await UniTask.Yield(destroyCancellationToken);
            }

            cachedTransform.position = targetPosition;
            isJumping = false;
        }
    }
}
