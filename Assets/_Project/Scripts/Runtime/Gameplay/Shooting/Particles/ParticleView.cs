using ControllThemAll.Runtime.Infrastructure;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Collider))]
    public class ParticleView : MonoBehaviour, IGameplayEntity
    {
        private int id;
        private string gameplayLayer;
        private IGameplayEntity parent;
        private Transform cachedTransform;
        private Vector3 movementDirection;

        private IMovementInput movementInput;
        private ParticleConfig particleConfig;
        private TimeScaleHolder timeScaleHolder;


        public int Id => id;
        public int PhysicsLayer => this == null ? 0 : gameObject.layer;
        public string GameplayLayer => gameplayLayer;
        public IGameplayEntity Parent => parent;


        public void Initialize(int id, TimeScaleHolder timeScaleHolder)
        {
            cachedTransform = GetComponent<Transform>();

            this.id = id;
            this.timeScaleHolder = timeScaleHolder;
        }


        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            Move();
        }


        public void SetParent(IGameplayEntity parent)
        {
            this.parent = parent;
        }

        public void SetGameplayLayer(string gameplayLayer)
        {
            this.gameplayLayer = gameplayLayer;
        }

        public void SetColor(Color color)
        {
            GetComponent<Renderer>().material.color = color;
        }

        public void SetMovementInput(IMovementInput movementInput)
        {
            this.movementInput = movementInput;
        }

        public void SetParticleConfig(ParticleConfig particleConfig)
        {
            this.particleConfig = particleConfig;
        }

        public ParticleConfig GetParticleConfig()
        {
            return particleConfig;
        }


        private void HandleInput()
        {
            movementDirection = movementInput?.Direction ?? Vector3.zero;
        }

        private void Move()
        {
            Vector3 newPosition = cachedTransform.position + movementDirection * particleConfig.Speed * Time.fixedDeltaTime * timeScaleHolder.GameplayTimeScale;

            cachedTransform.position = newPosition;
        }
    }
}