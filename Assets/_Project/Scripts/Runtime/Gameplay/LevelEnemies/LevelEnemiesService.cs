using ControllThemAll.Runtime.Infrastructure;
using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay
{
    public class LevelEnemiesService : ILoadingModule, IDisposable
    {
        public Action<EnemyView> EnemyDied;


        private readonly EnemyFactory enemyFactory;
        private readonly StatsService statsService;
        private readonly EnemiesConfigsContainer enemiesConfigsContainer;

        private List<EnemyView> enemies;
        private Dictionary<int, LevelEnemyConfig> enemiesConfigs;


        public IReadOnlyList<EnemyView> Enemies => enemies;


        public LevelEnemiesService(
            EnemyFactory enemyFactory,
            StatsService statsService,
            EnemiesConfigsContainer enemiesConfigsContainer)
        {
            this.enemyFactory = enemyFactory;
            this.statsService = statsService;
            this.enemiesConfigsContainer = enemiesConfigsContainer;

            enemies = new List<EnemyView>();
            enemiesConfigs = new Dictionary<int, LevelEnemyConfig>();
        }

        public UniTask Load()
        {
            foreach (LevelEnemyConfig levelEnemyConfig in enemiesConfigsContainer.GetLevelEnemiesConfigs())
            {
                EnemyConfig enemyConfig = enemiesConfigsContainer.GetEnemyConfig(levelEnemyConfig.EnemyId);
                EnemyView enemy = enemyFactory.CreateEnemy(enemyConfig);
                enemy.gameObject.transform.position = levelEnemyConfig.SpawnPoint;

                BindEnemyStats(enemy, levelEnemyConfig);

                enemies.Add(enemy);
                enemiesConfigs.Add(enemy.Id, levelEnemyConfig);
            }

            statsService.HealthStatChanged += OnHealthStatChanged;

            return UniTask.CompletedTask;
        }

        public void SetEnemiesActive(bool active)
        {
            if (active)
            {
                foreach (EnemyView enemy in enemies)
                {
                    enemy.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (EnemyView enemy in enemies)
                {
                    enemy.gameObject.SetActive(false);
                }
            }
        }

        public bool TryGetEnemyById(int id, out EnemyView enemy)
        {
            enemy = enemies.Find(levelEnemy => levelEnemy.Id == id);

            return enemy != null;
        }

        public bool TryGetEnemyConfigByEnemyId(int id, out LevelEnemyConfig enemyConfig)
        {
            return enemiesConfigs.TryGetValue(id, out enemyConfig);
        }

        public void Dispose()
        {
            statsService.HealthStatChanged -= OnHealthStatChanged;
        }


        private void OnHealthStatChanged(int entityId, float previousValue, float newValue)
        {
            if (!TryGetEnemyById(entityId, out EnemyView enemy))
            {
                return;
            }

            if (newValue > 0)
            {
                return;
            }

            EnemyDied?.Invoke(enemy);
            enemies.Remove(enemy);
            enemiesConfigs.Remove(enemy.Id);
            UnbindEnemyStats(enemy);
            UnityEngine.Object.Destroy(enemy.gameObject);
        }

        private void BindEnemyStats(EnemyView enemy, LevelEnemyConfig levelEnemyConfig)
        {
            StatsContainer enemyStatsContainer = new StatsContainer();

            enemyStatsContainer.AddStat(
                RuntimeConstants.Stats.Health,
                new FloatStat(levelEnemyConfig.MaxHealth, levelEnemyConfig.Health));

            statsService.BindStats(enemy.Id, enemyStatsContainer);
        }

        private void UnbindEnemyStats(EnemyView enemy)
        {
            statsService.ClearStats(enemy.Id);
        }
    }
}