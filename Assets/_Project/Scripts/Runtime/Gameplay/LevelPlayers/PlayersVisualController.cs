using System;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay
{
    public class PlayersVisualController : ILoadingModule, IDisposable
    {
        private readonly LevelPlayersService levelPlayersService;
        private readonly LevelPlayersWeaponsService levelPlayersWeaponsService;
        private readonly StatsService statsService;
        private readonly ShootingService shootingService;
        private readonly GameplayLayersConfigsContainer gameplayLayersConfigsContainer;
        private readonly VfxService vfxService;
        private readonly GameplayLayersService gameplayLayersService;


        public PlayersVisualController(
            LevelPlayersService levelPlayersService,
            LevelPlayersWeaponsService levelPlayersWeaponsService,
            StatsService statsService,
            ShootingService shootingService,
            GameplayLayersConfigsContainer gameplayLayersConfigsContainer,
            VfxService vfxService,
            GameplayLayersService gameplayLayersService)
        {
            this.levelPlayersService = levelPlayersService;
            this.levelPlayersWeaponsService = levelPlayersWeaponsService;
            this.statsService = statsService;
            this.shootingService = shootingService;
            this.gameplayLayersConfigsContainer = gameplayLayersConfigsContainer;
            this.vfxService = vfxService;
            this.gameplayLayersService = gameplayLayersService;
        }

        public UniTask Load()
        {
            statsService.HealthStatChanged += OnHealthStatChanged;
            shootingService.Fired += OnFired;
            gameplayLayersService.GameplayLayerChanged += OnGameplayLayerChanged;

            foreach (PlayerView player in levelPlayersService.Players)
            {
                SyncPlayerViewWithGameplayLayer(player);

                if (levelPlayersWeaponsService.TryGetWeaponByPlayerId(player.Id, out WeaponView weapon))
                {
                    SyncWeaponViewWithGameplayLayer(weapon);
                }
            }

            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            statsService.HealthStatChanged -= OnHealthStatChanged;
            shootingService.Fired -= OnFired;
            gameplayLayersService.GameplayLayerChanged -= OnGameplayLayerChanged;
        }


        private void OnFired(IGameplayEntity entity)
        {
            if (!levelPlayersService.TryGetPlayerById(entity.Parent?.Id ?? entity.Id, out PlayerView player))
            {
                return;
            }

            player.PlayShootAnimation();
        }

        private void OnHealthStatChanged(int entityId, float previousValue, float newValue)
        {
            if (!levelPlayersService.TryGetPlayerById(entityId, out PlayerView player))
            {
                return;
            }

            player.SetFillRate(CalculateFillRate(player.Id));

            if (newValue < previousValue)
            {
                PlayerGameplayLayerConfig playerGameplayLayerConfig =
                    gameplayLayersConfigsContainer.GetPlayerGameplayLayerConfig(player.GameplayLayer);

                player.PlayHitAnimation();
                vfxService.PlayEffect(VfxType.JellyDamageTake, player.transform.position, playerGameplayLayerConfig.FillColor);
            }
        }

        private void OnGameplayLayerChanged(IGameplayEntity entity, string previousGameplayLayer, string newGameplayLayer)
        {
            if (levelPlayersService.TryGetPlayerById(entity.Id, out PlayerView player))
            {
                SyncPlayerViewWithGameplayLayer(player);
                vfxService.PlayEffect(VfxType.MultiColorArea, player.transform.position, player.BoundsX / 2);
            }

            if (levelPlayersWeaponsService.TryGetWeaponById(entity.Id, out WeaponView weapon))
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

        private void SyncPlayerViewWithGameplayLayer(PlayerView player)
        {
            PlayerGameplayLayerConfig playerGameplayLayerConfig =
                gameplayLayersConfigsContainer.GetPlayerGameplayLayerConfig(player.GameplayLayer);

            player.SetFillColor(playerGameplayLayerConfig.FillColor);
            player.SetBaseColor(playerGameplayLayerConfig.BaseColor);
        }

        private void SyncWeaponViewWithGameplayLayer(WeaponView weapon)
        {
            DefaultGameplayLayerConfig defaultGameplayLayerConfig =
                gameplayLayersConfigsContainer.GetDefaultGameplayLayerConfig(weapon.GameplayLayer);

            weapon.SetColor(defaultGameplayLayerConfig.Color);
        }
    }
}