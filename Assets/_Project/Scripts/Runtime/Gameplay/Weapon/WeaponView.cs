using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class WeaponView : MonoBehaviour, IGameplayEntity
    {
        [SerializeField] private Ease showAnimationEase = Ease.OutBounce;
        [SerializeField] private float showAnimationDuration = 0.2f;
        [SerializeField] private Ease hideAnimationEase = Ease.Linear;
        [SerializeField] private float hideAnimationDuration = 0.2f;

        private TimeScaleHolder timeScaleHolder;

        private int id;
        private string gameplayLayer;
        private IGameplayEntity parent;
        private Vector3 initialLocalScale;
        private Quaternion initialLocalRotation;
        private Quaternion showInitialLocalRotation;
        private Quaternion hideFinalLocalRotation;
        private MotionHandle showHideHandle;


        public int Id => id;
        public int PhysicsLayer => this == null ? 0 : gameObject.layer;
        public string GameplayLayer => gameplayLayer;
        public IGameplayEntity Parent => parent;


        public void Initialize(int id, TimeScaleHolder timeScaleHolder)
        {
            this.id = id;
            this.timeScaleHolder = timeScaleHolder;

            initialLocalScale = transform.localScale;
            initialLocalRotation = transform.localRotation;
            showInitialLocalRotation = initialLocalRotation * Quaternion.Euler(0, 0, 90);
            hideFinalLocalRotation = initialLocalRotation * Quaternion.Euler(0, 0, -90);
        }


        private void Update()
        {
            if (showHideHandle.IsActive())
            {
                showHideHandle.PlaybackSpeed = timeScaleHolder.GameplayVisualEffectsTimeScale;
            }
        }


        public void SetColor(Color color)
        {
            Renderer renderer = GetComponent<Renderer>();

            if (renderer != null)
            {
                GetComponent<Renderer>().material.color = color;
            }
        }

        public void SetGameplayLayer(string gameplayLayer)
        {
            this.gameplayLayer = gameplayLayer;
        }

        public void SetParent(IGameplayEntity parent)
        {
            this.parent = parent;
        }

        public async UniTask PlayShowAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(Vector3.zero, initialLocalScale, showAnimationDuration)
                    .WithEase(showAnimationEase)
                    .BindToLocalScale(transform))
                .Join(LMotion.Create(showInitialLocalRotation, initialLocalRotation, showAnimationDuration)
                    .WithEase(showAnimationEase)
                    .BindToLocalRotation(transform))
                .Run();

            await showHideHandle;
        }

        public async UniTask PlayHideAnimation()
        {
            showHideHandle.TryComplete();

            showHideHandle = LSequence.Create()
                .Join(LMotion.Create(initialLocalScale, Vector3.zero, hideAnimationDuration)
                    .WithEase(hideAnimationEase)
                    .BindToLocalScale(transform))
                .Join(LMotion.Create(initialLocalRotation, hideFinalLocalRotation, showAnimationDuration)
                    .WithEase(showAnimationEase)
                    .BindToLocalRotation(transform))
                .Run();

            await showHideHandle;
        }
    }
}