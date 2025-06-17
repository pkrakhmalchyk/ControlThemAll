using System;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay
{
    public class EnemiesVisualController : ILoadingModule, IDisposable
    {
        private readonly LevelEnemiesService levelEnemiesService;
        private readonly LevelEnemiesWeaponsService levelEnemiesWeaponsService;
        private readonly StatsService statsService;
        private readonly GameplayLayersConfigsContainer gameplayLayersConfigsContainer;
        private readonly VfxService vfxService;
        private readonly GameplayLayersService gameplayLayersService;


        public EnemiesVisualController(
            LevelEnemiesService levelEnemiesService,
            LevelEnemiesWeaponsService levelEnemiesWeaponsService,
            StatsService statsService,
            GameplayLayersConfigsContainer gameplayLayersConfigsContainer,
            VfxService vfxService,
            GameplayLayersService gameplayLayersService)
        {
            this.levelEnemiesService = levelEnemiesService;
            this.levelEnemiesWeaponsService = levelEnemiesWeaponsService;
            this.statsService = statsService;
            this.gameplayLayersConfigsContainer = gameplayLayersConfigsContainer;
            this.vfxService = vfxService;
            this.gameplayLayersService = gameplayLayersService;
        }

        public UniTask Load()
        {
            statsService.HealthStatChanged += OnHealthStatChanged;
            gameplayLayersService.GameplayLayerChanged += OnGameplayLayerChanged;

            foreach (EnemyView enemy in levelEnemiesService.Enemies)
            {
                SyncEnemyViewWithGameplayLayer(enemy);

                if (levelEnemiesWeaponsService.TryGetWeaponByEnemyId(enemy.Id, out WeaponView weapon))
                {
                    SyncWeaponViewWithGameplayLayer(weapon);
                }
            }

            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            statsService.HealthStatChanged += OnHealthStatChanged;
            gameplayLayersService.GameplayLayerChanged += OnGameplayLayerChanged;
        }


        private void OnHealthStatChanged(int entityId, float previousValue, float newValue)
        {
            if (!levelEnemiesService.TryGetEnemyById(entityId, out EnemyView enemy))
            {
                return;
            }

            enemy.SetFillRate(CalculateFillRate(enemy.Id));

            if (newValue < previousValue)
            {
                EnemyGameplayLayerConfig enemyGameplayLayerConfig =
                    gameplayLayersConfigsContainer.GetEnemyGameplayLayerConfig(enemy.GameplayLayer);

                enemy.PlayHitAnimation();
                vfxService.PlayEffect(VfxType.JellyDamageTake, enemy.transform.position, enemyGameplayLayerConfig.FillColor);
            }
        }

        private void OnGameplayLayerChanged(IGameplayEntity entity, string previousGameplayLayer, string newGameplayLayer)
        {
            if (levelEnemiesService.TryGetEnemyById(entity.Id, out EnemyView enemy))
            {
                SyncEnemyViewWithGameplayLayer(enemy);
                vfxService.PlayEffect(VfxType.MultiColorArea, enemy.transform.position, enemy.BoundsX / 2);
            }

            if (levelEnemiesWeaponsService.TryGetWeaponById(entity.Id, out WeaponView weapon))
            {
                SyncWeaponViewWithGameplayLayer(weapon);
            }
        }

        private float CalculateFillRate(int entityId)
        {
            StatsContainer statsContainer = statsService.GetStats(entityId);

            if (statsContainer == null)
            {
                return 0;
            }

            if (!statsContainer.TryGetStat(RuntimeConstants.Stats.Health, out IStat<float> healthStat))
            {
                return 0;
            }

            return healthStat.Value / healthStat.MaxValue;
        }

        private void SyncEnemyViewWithGameplayLayer(EnemyView enemy)
        {
            EnemyGameplayLayerConfig enemyGameplayLayerConfig =
                gameplayLayersConfigsContainer.GetEnemyGameplayLayerConfig(enemy.GameplayLayer);

            enemy.SetFillColor(enemyGameplayLayerConfig.FillColor);
            enemy.SetBaseColor(enemyGameplayLayerConfig.BaseColor);
        }

        private void SyncWeaponViewWithGameplayLayer(WeaponView weapon)
        {
            DefaultGameplayLayerConfig defaultGameplayLayerConfig =
                gameplayLayersConfigsContainer.GetDefaultGameplayLayerConfig(weapon.GameplayLayer);

            weapon.SetColor(defaultGameplayLayerConfig.Color);
        }
    }
}