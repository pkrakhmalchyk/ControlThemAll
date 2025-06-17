using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class ParticleContext : IBoundsContext, ITransformContext
    {
        private readonly ParticleView particle;
        private readonly Collider collider;
        private readonly Transform transform;


        public IGameplayEntity Owner => particle;
        public Vector3 Bounds => collider.bounds.size;
        public Vector3 Position => transform.position;
        public Quaternion Orientation => transform.rotation;
        public Transform Transform => transform;


        public ParticleContext(ParticleView particle)
        {
            this.particle = particle;
            collider = particle.GetComponent<Collider>();
            transform = particle.transform;
        }
    }
}