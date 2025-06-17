using System;
using System.Collections.Generic;
using System.Linq;
using ControllThemAll.Runtime.Infrastructure;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay
{
    public class LevelPlayersWeaponsService : ILoadingModule, IDisposable
    {
        public Action WeaponChanged;


        private readonly InventorySystem<PlayerWeaponConfig> inventorySystem;
        private readonly ShootingService shootingService;
        private readonly AbilitiesService abilitiesService;
        private readonly WeaponFactory weaponFactory;
        private readonly PlayersConfigsContainer playersConfigsContainer;
        private readonly LevelPlayersService levelPlayersService;
        private readonly DragAttackInput attackInput;

        private Dictionary<int, WeaponView> playersWeapons;
        private PlayerWeaponConfig currentWeapon;


        public PlayerWeaponConfig CurrentWeapon => currentWeapon;


        public LevelPlayersWeaponsService(
            InventorySystem<PlayerWeaponConfig> inventorySystem,
            ShootingService shootingService,
            AbilitiesService abilitiesService,
            WeaponFactory weaponFactory,
            DragAttackInput attackInput,
            PlayersConfigsContainer playersConfigsContainer,
            LevelPlayersService levelPlayersService)
        {
            this.inventorySystem = inventorySystem;
            this.shootingService = shootingService;
            this.abilitiesService = abilitiesService;
            this.weaponFactory = weaponFactory;
            this.playersConfigsContainer = playersConfigsContainer;
            this.levelPlayersService = levelPlayersService;
            this.attackInput = attackInput;

            playersWeapons = new Dictionary<int, WeaponView>();
        }

        public UniTask Load()
        {
            FillInventorySystem();

            PlayerWeaponConfig playerWeaponConfig = inventorySystem.GetValueWithMinCount(levelPlayersService.Players.Count);

            SetCurrentWeaponInternal(playerWeaponConfig);
            WeaponChanged?.Invoke();

            shootingService.Fired += OnFired;
            levelPlayersService.PlayerDied += OnPlayerDied;

            return UniTask.CompletedTask;
        }

        public void SetWeaponsActive(bool active)
        {
            foreach (WeaponView playerWeapon in playersWeapons.Values)
            {
                playerWeapon.gameObject.SetActive(active);
                abilitiesService.SetAbilitiesActive(playerWeapon.Id, active);

                if (active)
                {
                    playerWeapon.PlayShowAnimation().Forget();
                }
                else
                {
                    playerWeapon.PlayHideAnimation().Forget();
                }
            }
        }

        public void SetCurrentWeapon(PlayerWeaponConfig weaponConfig)
        {
            RemoveCurrentWeapon().Forget();

            bool isWeaponInInventory = inventorySystem.GetCount(weaponConfig) >= levelPlayersService.Players.Count;

            if (!isWeaponInInventory)
            {
                return;
            }

            SetCurrentWeaponInternal(weaponConfig);
            SetWeaponsActive(true);

            WeaponChanged?.Invoke();
        }

        public bool TryGetWeaponByPlayerId(int playerId, out WeaponView weapon)
        {
            return playersWeapons.TryGetValue(playerId, out weapon);
        }

        public bool TryGetWeaponById(int id, out WeaponView weapon)
        {
            foreach (WeaponView existingWeapon in playersWeapons.Values)
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
            shootingService.Fired -= OnFired;
            levelPlayersService.PlayerDied -= OnPlayerDied;
        }


        private void OnFired(IGameplayEntity entity)
        {
            if (!TryGetWeaponById(entity.Id, out WeaponView weapon))
            {
                return;
            }

            inventorySystem.Remove(currentWeapon);

            if (inventorySystem.GetCount(currentWeapon) > 0)
            {
                return;
            }

            RemoveCurrentWeapon().Forget();

            PlayerWeaponConfig nextPlayerWeaponConfig = inventorySystem.GetValueWithMinCount(levelPlayersService.Players.Count);

            if (nextPlayerWeaponConfig == null)
            {
                return;
            }

            SetCurrentWeaponInternal(nextPlayerWeaponConfig);
            SetWeaponsActive(true);

            WeaponChanged?.Invoke();
        }

        private void OnPlayerDied(PlayerView player)
        {
            if (playersWeapons.Remove(player.Id, out WeaponView weapon))
            {
                DestroyPlayerWeapon(weapon);
            }
        }

        private async UniTaskVoid RemoveCurrentWeapon()
        {
            List<WeaponView> weapons = new List<WeaponView>();

            foreach (PlayerView player in levelPlayersService.Players)
            {
                if (playersWeapons.Remove(player.Id, out WeaponView weapon))
                {
                    weapons.Add(weapon);
                    abilitiesService.ClearAbilities(weapon.Id);
                }
            }

            currentWeapon = null;

            await weapons.Select(weapon => weapon.PlayHideAnimation());

            foreach (WeaponView weapon in weapons)
            {
                DestroyPlayerWeapon(weapon);
            }
        }

        private void SetCurrentWeaponInternal(PlayerWeaponConfig playerWeaponConfig)
        {
            if (playerWeaponConfig == null)
            {
                return;
            }

            attackInput.SetSensitivity(playerWeaponConfig.InputSensitivity);
            attackInput.SetMaxForceHorizontalAngle(playerWeaponConfig.MaxAimHorizontalAngle);

            foreach (PlayerView player in levelPlayersService.Players)
            {
                WeaponView weapon = weaponFactory.CreateWeapon(playerWeaponConfig.WeaponConfig, true);

                weapon.transform.SetParent(player.WeaponSlot, false);
                weapon.SetParent(player);

                BindWeaponAbilities(weapon, player, playerWeaponConfig);
                playersWeapons.Add(player.Id, weapon);
            }

            currentWeapon = playerWeaponConfig;
        }

        private void FillInventorySystem()
        {
            foreach (LevelPlayerWeaponConfig levelWeaponConfig in playersConfigsContainer.GetLevelPlayerWeaponsConfigs())
            {
                PlayerWeaponConfig playerWeaponConfig = playersConfigsContainer.GetPlayerWeaponConfig(levelWeaponConfig.WeaponId);

                inventorySystem.Add(playerWeaponConfig, levelWeaponConfig.Count);
            }
        }

        private void DestroyPlayerWeapon(WeaponView weapon)
        {
            UnbindWeaponAbilities(weapon);
            UnityEngine.Object.Destroy(weapon.gameObject);
        }

        private void BindWeaponAbilities(WeaponView weapon, PlayerView player, PlayerWeaponConfig playerWeaponConfig)
        {
            PlayerWeaponContext playerWeaponContext = new PlayerWeaponContext(
                weapon,
                player,
                attackInput);
            IEnumerable<AbilityConfig<PlayerWeaponContext>> abilitiesConfigs = playerWeaponConfig.AbilitiesIds
                .Select(id => playersConfigsContainer.GetPlayerWeaponAbilityConfig(id));

            abilitiesService.BindAbilities(abilitiesConfigs, playerWeaponContext);
        }

        private void UnbindWeaponAbilities(WeaponView weapon)
        {
            abilitiesService.ClearAbilities(weapon.Id);
        }
    }
}