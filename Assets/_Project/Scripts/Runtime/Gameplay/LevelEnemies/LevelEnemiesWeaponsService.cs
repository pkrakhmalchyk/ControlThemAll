using System;
using System.Collections.Generic;
using System.Linq;
using ControllThemAll.Runtime.Infrastructure;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace ControllThemAll.Runtime.Gameplay
{
    public class LevelEnemiesWeaponsService : ILoadingModule, IDisposable
    {
        private readonly LevelEnemiesService levelEnemiesService;
        private readonly EnemiesConfigsContainer enemiesConfigsContainer;
        private readonly WeaponFactory weaponFactory;
        private readonly AbilitiesService abilitiesService;

        private Dictionary<int, WeaponView> enemiesWeapons;


        public LevelEnemiesWeaponsService(
            LevelEnemiesService levelEnemiesService,
            EnemiesConfigsContainer enemiesConfigsContainer,
            WeaponFactory weaponFactory,
            AbilitiesService abilitiesService)
        {
            this.levelEnemiesService = levelEnemiesService;
            this.enemiesConfigsContainer = enemiesConfigsContainer;
            this.weaponFactory = weaponFactory;
            this.abilitiesService = abilitiesService;

            enemiesWeapons = new Dictionary<int, WeaponView>();
        }

        public UniTask Load()
        {
            foreach (EnemyView enemy in levelEnemiesService.Enemies)
            {
                if (levelEnemiesService.TryGetEnemyConfigByEnemyId(enemy.Id, out LevelEnemyConfig levelEnemyConfig))
                {
                    EnemyWeaponConfig enemyWeaponConfig = enemiesConfigsContainer.GetEnemyWeaponConfig(levelEnemyConfig.WeaponId);
                    WeaponView enemyWeapon = CreateLevelEnemyWeapon(enemy, enemyWeaponConfig);

                    enemiesWeapons.Add(enemy.Id, enemyWeapon);
                }
            }

            levelEnemiesService.EnemyDied += OnEnemyDied;

            return UniTask.CompletedTask;
        }

        public void SetWeaponsActive(bool active)
        {
            foreach (WeaponView enemyWeapon in enemiesWeapons.Values)
            {
                enemyWeapon.gameObject.SetActive(active);
                abilitiesService.SetAbilitiesActive(enemyWeapon.Id, active);

                if (active)
                {
                    enemyWeapon.PlayShowAnimation().Forget();
                }
                else
                {
                    enemyWeapon.PlayHideAnimation().Forget();
                }
            }
        }

        public bool TryGetWeaponByEnemyId(int enemyId, out WeaponView weapon)
        {
            return enemiesWeapons.TryGetValue(enemyId, out weapon);
        }

        public bool TryGetWeaponById(int id, out WeaponView weapon)
        {
            foreach (WeaponView existingWeapon in enemiesWeapons.Values)
            {
                if (existingWeapon.Id == id)
                {
                    weapon = existingWeapon;

                    return true;
                }
            }

            weapon = null;

            return false;
        }

        public void Dispose()
        {
            levelEnemiesService.EnemyDied -= OnEnemyDied;
        }


        private void OnEnemyDied(EnemyView enemy)
        {
            if (enemiesWeapons.Remove(enemy.Id, out WeaponView weapon))
            {
                DestroyLevelEnemyWeapon(weapon);
            }
        }

        private WeaponView CreateLevelEnemyWeapon(EnemyView enemy, EnemyWeaponConfig enemyWeaponConfig)
        {
            WeaponView weapon = weaponFactory.CreateWeapon(enemyWeaponConfig.WeaponConfig,false);

            weapon.transform.SetParent(enemy.WeaponSlot, false);
            weapon.SetParent(enemy);

            BindWeaponAbilities(weapon, enemyWeaponConfig, enemy);

            return weapon;
        }

        private void DestroyLevelEnemyWeapon(WeaponView weapon)
        {
            UnbindWeaponAbilities(weapon);
            Object.Destroy(weapon.gameObject);
        }

        private void BindWeaponAbilities(WeaponView weapon, EnemyWeaponConfig enemyWeaponConfig, EnemyView enemy)
        {
            EnemyWeaponContext enemyWeaponContext = new EnemyWeaponContext(
                weapon,
                enemy);
            IEnumerable<AbilityConfig<EnemyWeaponContext>> abilitiesConfigs = enemyWeaponConfig.AbilitiesIds
                .Select(id => enemiesConfigsContainer.GetEnemyWeaponAbilityConfig(id));

            abilitiesService.BindAbilities(abilitiesConfigs, enemyWeaponContext);
        }

        private void UnbindWeaponAbilities(WeaponView weapon)
        {
            abilitiesService.ClearAbilities(weapon.Id);
        }
    }
}