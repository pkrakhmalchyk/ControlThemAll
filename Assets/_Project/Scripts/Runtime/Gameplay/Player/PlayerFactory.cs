using System;
using ControllThemAll.Runtime.Infrastructure;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using Object = UnityEngine.Object;

namespace ControllThemAll.Runtime.Gameplay
{
    public class PlayerFactory : ILoadingModule, IDisposable
    {
        private readonly IObjectResolver container;

        private PlayerView defaultPlayerPrefab;
        private AsyncOperationHandle<GameObject> defaultPlayerPrefabHandle;


        public PlayerFactory(IObjectResolver container)
        {
            this.container = container;
        }

        public async UniTask Load()
        {
            await LoadPrefabs();
        }

        public PlayerView CreatePlayer(PlayerConfig playerConfig)
        {
            PlayerView player = Object.Instantiate(defaultPlayerPrefab);
            player.gameObject.layer = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.Player);
            player.gameObject.SetActive(false);

            player.Initialize(
                container.Resolve<IdGenerator>().GetNextId(),
                playerConfig,
                new ConstantDirectionalMovementInput(Vector3.forward),
                container.Resolve<TimeScaleHolder>());

            return player;
        }

        public void Dispose()
        {
            Addressables.Release(defaultPlayerPrefabHandle);
        }


        private async UniTask LoadPrefabs()
        {
            defaultPlayerPrefabHandle = Addressables.LoadAssetAsync<GameObject>($"{RuntimeConstants.Players.PlayersPath}/{RuntimeConstants.Players.DefaultPlayer}");
            defaultPlayerPrefab = (await defaultPlayerPrefabHandle).GetComponent<PlayerView>();
        }
    }
}
