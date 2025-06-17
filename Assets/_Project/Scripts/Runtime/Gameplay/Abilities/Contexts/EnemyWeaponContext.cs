using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class EnemyWeaponContext : IParentBoundsContext, ITransformContext
    {
        private readonly WeaponView weapon;
        private readonly EnemyView enemy;
        private readonly Collider enemyCollider;
        private readonly Transform enemyTransform;
        private readonly Transform weaponTransform;


        public IGameplayEntity Owner => weapon;
        Vector3 IParentBoundsContext.Bounds => enemyCollider.bounds.size;
        Vector3 IParentBoundsContext.Position => enemyTransform.position;
        Quaternion IParentBoundsContext.Orientation => enemyTransform.rotation;
        public Transform Transform => weaponTransform;


        public EnemyWeaponContext(WeaponView weapon, EnemyView enemy)
        {
            this.weapon = weapon;
            this.enemy = enemy;

            enemyCollider = enemy.GetComponent<Collider>();
            enemyTransform = enemy.transform;
            weaponTransform = weapon.transform;
        }
    }
}