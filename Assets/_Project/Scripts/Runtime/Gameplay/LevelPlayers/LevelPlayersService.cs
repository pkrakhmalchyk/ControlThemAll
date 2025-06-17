using ControllThemAll.Runtime.Infrastructure;
using System;
using System.Collections.Generic;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay
{
    public class LevelPlayersService : ILoadingModule, IDisposable
    {
        public Action<PlayerView> PlayerDied;


        private readonly StatsService statsService;
        private readonly PlayerFactory playerFactory;
        private readonly PlayersConfigsContainer playersConfigsContainer;

        private List<PlayerView> players;
        private Dictionary<int, LevelPlayerConfig> playersConfigs;


        public IReadOnlyList<PlayerView> Players => players;


        public LevelPlayersService(
            StatsService statsService,
            PlayerFactory playerFactory,
            PlayersConfigsContainer playersConfigsContainer)
        {
            this.statsService = statsService;
            this.playerFactory = playerFactory;
            this.playersConfigsContainer = playersConfigsContainer;

            players = new List<PlayerView>();
            playersConfigs = new Dictionary<int, LevelPlayerConfig>();
        }

        public UniTask Load()
        {
            IReadOnlyList<LevelPlayerConfig> levelPlayersConfigs = playersConfigsContainer.GetLevelPlayersConfigs();

            foreach (LevelPlayerConfig levelPlayerConfig in levelPlayersConfigs)
            {
                PlayerView player = playerFactory.CreatePlayer(playersConfigsContainer.GetPlayerConfig());
                player.gameObject.transform.position = levelPlayerConfig.SpawnPoint;

                BindPlayerStats(player, levelPlayerConfig);
                players.Add(player);
                playersConfigs.Add(player.Id, levelPlayerConfig);
            }

            statsService.HealthStatChanged += OnHealthStatChanged;

            return UniTask.CompletedTask;
        }

        public void SetPlayersActive(bool active)
        {
            if (active)
            {
                foreach (PlayerView player in players)
                {
                    player.gameObject.SetActive(true);
                }
            }
            else
            {
                foreach (PlayerView player in players)
                {
                    player.gameObject.SetActive(false);
                }
            }
        }

        public bool TryGetPlayerById(int id, out PlayerView player)
        {
            player = players.Find(levelPlayer => levelPlayer.Id == id);

            return player != null;
        }

        public bool TryGetPlayerConfigByPlayerId(int id, out LevelPlayerConfig enemyConfig)
        {
            return playersConfigs.TryGetValue(id, out enemyConfig);
        }

        public void Dispose()
        {
            statsService.HealthStatChanged -= OnHealthStatChanged;
        }


        private void OnHealthStatChanged(int entityId, float previousValue, float newValue)
        {
            if (!TryGetPlayerById(entityId, out PlayerView player))
            {
                return;
            }

            if (newValue > 0)
            {
                return;
            }

            PlayerDied?.Invoke(player);
            players.Remove(player);
            playersConfigs.Remove(player.Id);
            UnbindPlayerStats(player);
            UnityEngine.Object.Destroy(player.gameObject);
        }

        private void BindPlayerStats(PlayerView player, LevelPlayerConfig levelPlayerConfig)
        {
            StatsContainer playerStatsContainer = new StatsContainer();

            playerStatsContainer.AddStat(
                RuntimeConstants.Stats.Health,
                new FloatStat(levelPlayerConfig.MaxHealth, levelPlayerConfig.Health));

            statsService.BindStats(player.Id, playerStatsContainer);
        }

        private void UnbindPlayerStats(PlayerView player)
        {
            statsService.ClearStats(player.Id);
        }
    }
}
