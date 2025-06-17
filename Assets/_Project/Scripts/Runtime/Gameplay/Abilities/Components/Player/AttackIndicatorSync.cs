using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackIndicatorSync : IAbilityComponent, IDisposable
    {
        private readonly AttackIndicatorSyncConfig config;
        private readonly PlayerWeaponContext context;
        private CancellationTokenSource cts;


        public AttackIndicatorSync(
            AttackIndicatorSyncConfig config,
            PlayerWeaponContext context)
        {
            this.config = config;
            this.context = context;
        }


        public void Execute(bool execute)
        {
            if (execute)
            {
                StartSync();
            }
            else
            {
                FinishSync();
            }
        }

        public void Dispose()
        {
            FinishSync();
        }


        private void StartSync()
        {
            cts = new CancellationTokenSource();

            SyncAttackIndicator(cts.Token).Forget();
        }

        private void FinishSync()
        {
            if (context.Player != null)
            {
                context.Player.PlayHideAttackIndicatorAnimation().Forget();
            }

            cts?.Cancel();
            cts?.Dispose();

            cts = null;
        }

        private async UniTask SyncAttackIndicator(CancellationToken cancellationToken)
        {
            await context.Player.PlayShowAttackIndicatorAnimation();

            while (!cancellationToken.IsCancellationRequested)
            {
                float attackIndicatorScale = context.AttackInput.Force.magnitude / config.MinAttackForce;

                context.Player.SetAttackIndicatorScale(attackIndicatorScale);

                await UniTask.Yield();
            }
        }
    }
}
