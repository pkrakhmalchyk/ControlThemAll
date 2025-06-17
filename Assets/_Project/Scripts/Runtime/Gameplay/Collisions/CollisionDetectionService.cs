using ControllThemAll.Runtime.Utils;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class CollisionDetectionService
    {
        public bool IsCollisionInBoxDetected(Vector3 position, Vector3 bounds, Quaternion orientation, int physicsLayer)
        {
            LayerMask collisionLayerMask = GetCollisionLayerMask(physicsLayer);
            Vector3 halfExtents = bounds / 2;

            return Physics.CheckBox(position, halfExtents, orientation, collisionLayerMask);
        }

        public Collider[] GetCollisionsInBox(Vector3 position, Vector3 bounds, Quaternion orientation, int physicsLayer)
        {
            LayerMask collisionLayerMask = GetCollisionLayerMask(physicsLayer);
            Vector3 halfExtents = bounds / 2;

            return Physics.OverlapBox(position, halfExtents, orientation, collisionLayerMask);
        }


        private LayerMask GetCollisionLayerMask(int layer)
        {
            string layerName = LayerMask.LayerToName(layer);
            string[] collisionLayers = layerName switch
            {
                RuntimeConstants.PhysicLayers.EnemyParticle => RuntimeConstants.PhysicLayers.EnemyParticleCollisionLayers,
                RuntimeConstants.PhysicLayers.EnemyWeapon => RuntimeConstants.PhysicLayers.EnemyWeaponCollisionLayers,
                RuntimeConstants.PhysicLayers.PlayerParticle => RuntimeConstants.PhysicLayers.PlayerParticleCollisionLayers,
                RuntimeConstants.PhysicLayers.PlayerWeapon => RuntimeConstants.PhysicLayers.PlayerWeaponCollisionLayers,
                RuntimeConstants.PhysicLayers.EnvironmentPart => RuntimeConstants.PhysicLayers.EnvironmentPartCollisionLayers,
                _ => new string[] { }
            };

            return LayerMask.GetMask(collisionLayers);
        }
    }
}