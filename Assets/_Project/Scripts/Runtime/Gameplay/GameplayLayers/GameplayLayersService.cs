using System;
using System.Collections.Generic;
using System.Threading;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay
{
    public class GameplayLayersService : ILoadingModule, IDisposable
    {
        public Action<IGameplayEntity, string, string> GameplayLayerChanged;


        private readonly LevelPlayersService levelPlayersService;
        private readonly LevelPlayersWeaponsService levelPlayersWeaponsService;
        private readonly LevelEnemiesService levelEnemiesService;
        private readonly LevelEnemiesWeaponsService levelEnemiesWeaponsService;
        private readonly TimeScaleHolder timeScaleHolder;

        private readonly Dictionary<int, TemporaryGameplayLayerChange> temporaryEntitiesGameplayLayersChanges;


        public GameplayLayersService(
            LevelPlayersService levelPlayersService,
            LevelPlayersWeaponsService levelPlayersWeaponsService,
            LevelEnemiesService levelEnemiesService,
            LevelEnemiesWeaponsService levelEnemiesWeaponsService,
            TimeScaleHolder timeScaleHolder)
        {
            this.levelPlayersService = levelPlayersService;
            this.levelPlayersWeaponsService = levelPlayersWeaponsService;
            this.levelEnemiesService = levelEnemiesService;
            this.levelEnemiesWeaponsService = levelEnemiesWeaponsService;
            this.timeScaleHolder = timeScaleHolder;

            temporaryEntitiesGameplayLayersChanges = new Dictionary<int, TemporaryGameplayLayerChange>();
        }

        public UniTask Load()
        {
            foreach (PlayerView player in levelPlayersService.Players)
            {
                if (levelPlayersService.TryGetPlayerConfigByPlayerId(player.Id, out LevelPlayerConfig config))
                {
                    SetGameplayLayerInternal(player, config.GameplayLayer);
                }
            }

            foreach (EnemyView enemy in levelEnemiesService.Enemies)
            {
                if (levelEnemiesService.TryGetEnemyConfigByEnemyId(enemy.Id, out LevelEnemyConfig config))
                {
                    SetGameplayLayerInternal(enemy, config.GameplayLayer);
                }
            }

            levelPlayersWeaponsService.WeaponChanged += OnPlayerWeaponChanged;

            return UniTask.CompletedTask;
        }

        public void SetGameplayLayer(IGameplayEntity target, string gameplayLayer, float duration)
        {
            if (target.GameplayLayer == gameplayLayer)
            {
                return;
            }

            TemporaryGameplayLayerChange previousTemporaryGameplayLayerChange = CancelTemporaryGameplayLayerChangeIfExist(target);
            string entityOriginalGameplayLayer = previousTemporaryGameplayLayerChange != null
                ? previousTemporaryGameplayLayerChange.OriginalGameplayLayer
                : target.GameplayLayer;
            TemporaryGameplayLayerChange temporaryGameplayLayerChange = new TemporaryGameplayLayerChange(
                target,
                new CancellationTokenSource(),
                entityOriginalGameplayLayer);

            temporaryEntitiesGameplayLayersChanges.Add(target.Id, temporaryGameplayLayerChange);
            SetTemporaryGameplayLayer(temporaryGameplayLayerChange, gameplayLayer, duration).Forget();
        }

        public void Dispose()
        {
            levelPlayersWeaponsService.WeaponChanged -= OnPlayerWeaponChanged;

            foreach (KeyValuePair<int, TemporaryGameplayLayerChange> change in temporaryEntitiesGameplayLayersChanges)
            {
                change.Value.Cts.Cancel();
                change.Value.Cts.Dispose();
            }

            temporaryEntitiesGameplayLayersChanges.Clear();
        }


        private void OnPlayerWeaponChanged()
        {
            foreach (PlayerView player in levelPlayersService.Players)
            {
                if (levelPlayersWeaponsService.TryGetWeaponByPlayerId(player.Id, out WeaponView weapon))
                {
                    SetWeaponGameplayLayer(weapon, player.GameplayLayer);
                }
            }
        }

        private async UniTaskVoid SetTemporaryGameplayLayer(TemporaryGameplayLayerChange gameplayLayerChange, string gameplayLayer, float duration)
        {
            SetGameplayLayerInternal(gameplayLayerChange.Entity, gameplayLayer);

            await UniTaskHelper.DelayWithGameplayTimeScale(duration, timeScaleHolder, gameplayLayerChange.Cts.Token);

            SetGameplayLayerInternal(gameplayLayerChange.Entity, gameplayLayerChange.OriginalGameplayLayer);

            gameplayLayerChange.Cts.Dispose();
            temporaryEntitiesGameplayLayersChanges.Remove(gameplayLayerChange.Entity.Id);
        }

        private TemporaryGameplayLayerChange CancelTemporaryGameplayLayerChangeIfExist(IGameplayEntity entity)
        {
            if (temporaryEntitiesGameplayLayersChanges.TryGetValue(entity.Id,
                    out TemporaryGameplayLayerChange temporaryGameplayLayerChange))
            {
                temporaryGameplayLayerChange.Cts.Cancel();
                temporaryGameplayLayerChange.Cts.Dispose();

                temporaryEntitiesGameplayLayersChanges.Remove(entity.Id);
            }

            return temporaryGameplayLayerChange;
        }

        private void SetGameplayLayerInternal(IGameplayEntity target, string gameplayLayer)
        {
            if (levelPlayersService.TryGetPlayerById(target.Id, out PlayerView player))
            {
                SetPlayerGameplayLayer(player, gameplayLayer);
            }
            else if (levelEnemiesService.TryGetEnemyById(target.Id, out EnemyView enemy))
            {
                SetEnemyGameplayLayer(enemy, gameplayLayer);
            }
        }

        private void SetPlayerGameplayLayer(PlayerView player, string gameplayLayer)
        {
            string previousPlayerGameplayLayer = player.GameplayLayer;

            player.SetGameplayLayer(gameplayLayer);
            GameplayLayerChanged?.Invoke(player, previousPlayerGameplayLayer, gameplayLayer);

            if (levelPlayersWeaponsService.TryGetWeaponByPlayerId(player.Id, out WeaponView weapon))
            {
                SetWeaponGameplayLayer(weapon, gameplayLayer);
            }
        }

        private void SetEnemyGameplayLayer(EnemyView enemy, string gameplayLayer)
        {
            string previousEnemyGameplayLayer = enemy.GameplayLayer;

            enemy.SetGameplayLayer(gameplayLayer);
            GameplayLayerChanged?.Invoke(enemy, previousEnemyGameplayLayer, gameplayLayer);

            if (levelEnemiesWeaponsService.TryGetWeaponByEnemyId(enemy.Id, out WeaponView weapon))
            {
                SetWeaponGameplayLayer(weapon, gameplayLayer);
            }
        }

        private void SetWeaponGameplayLayer(WeaponView weapon, string gameplayLayer)
        {
            string previousWeaponGameplayLayer = weapon.GameplayLayer;

            weapon.SetGameplayLayer(gameplayLayer);
            GameplayLayerChanged?.Invoke(weapon, previousWeaponGameplayLayer, gameplayLayer);
        }

        private class TemporaryGameplayLayerChange
        {
            public IGameplayEntity Entity;
            public CancellationTokenSource Cts;
            public string OriginalGameplayLayer;

            public TemporaryGameplayLayerChange(
                IGameplayEntity entity,
                CancellationTokenSource cts,
                string originalGameplayLayer)
            {
                Entity = entity;
                Cts = cts;
                OriginalGameplayLayer = originalGameplayLayer;
            }
        }
    }

}