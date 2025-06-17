using ControllThemAll.Runtime.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackTargetsProvider
    {
        public IEnumerable<IGameplayEntity> GetAttackTargetsInBox(Vector3 position, Vector3 bounds, Quaternion orientation, int physicsLayer, string gameplayLayer)
        {
            LayerMask attackLayerMask = GetAttackLayerMask(physicsLayer);
            Vector3 halfExtents = bounds / 2;
            Collider[] colliders = Physics.OverlapBox(position, halfExtents, orientation, attackLayerMask);

            return GetGameplayEntitiesWithAttackableGameplayLayers(colliders, gameplayLayer);
        }

        public IEnumerable<IGameplayEntity> GetAttackTargetsInSphere(Vector3 position, float radius, int physicsLayer, string gameplayLayer)
        {
            LayerMask attackLayerMask = GetAttackLayerMask(physicsLayer);
            Collider[] colliders = Physics.OverlapSphere(position, radius, attackLayerMask);

            return GetGameplayEntitiesWithAttackableGameplayLayers(colliders, gameplayLayer);
        }


        private IEnumerable<IGameplayEntity> GetGameplayEntitiesWithAttackableGameplayLayers(Collider[] colliders, string gameplayLayer)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.TryGetComponent(out IGameplayEntity gameplayEntity)
                    && gameplayEntity.GameplayLayer != gameplayLayer)
                {
                    yield return gameplayEntity;
                }
            }
        }

        private LayerMask GetAttackLayerMask(int physicsLayer)
        {
            string layerName = LayerMask.LayerToName(physicsLayer);
            string[] collisionLayers = layerName switch
            {
                RuntimeConstants.PhysicLayers.EnemyParticle => RuntimeConstants.PhysicLayers.EnemyAttackLayers,
                RuntimeConstants.PhysicLayers.EnemyWeapon => RuntimeConstants.PhysicLayers.EnemyAttackLayers,
                RuntimeConstants.PhysicLayers.PlayerParticle => RuntimeConstants.PhysicLayers.PlayerAttackLayers,
                RuntimeConstants.PhysicLayers.PlayerWeapon => RuntimeConstants.PhysicLayers.PlayerAttackLayers,
                _ => new string[] { }
            };

            return LayerMask.GetMask(collisionLayers);
        }
    }
}