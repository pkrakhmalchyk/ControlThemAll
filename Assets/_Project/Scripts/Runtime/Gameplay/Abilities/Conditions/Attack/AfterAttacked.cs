using System;
using System.Threading;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using VContainer;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AfterAttacked : IAbilityCondition, IDisposable
    {
        private readonly AfterAttackedConfig config;
        private readonly IAbilityContext context;
        private AttackService attackService;
        private TimeScaleHolder timeScaleHolder;

        private CancellationTokenSource cts;
        private bool isAttacked = false;


        public AfterAttacked(
            AfterAttackedConfig config,
            IAbilityContext context)
        {
            this.config = config;
            this.context = context;
        }

        [Inject]
        public void Initialize(
            AttackService attackService,
            TimeScaleHolder timeScaleHolder)
        {
            this.attackService = attackService;
            this.timeScaleHolder = timeScaleHolder;

            attackService.Attacked += OnAttacked;
        }

        public bool IsFulfilled()
        {
            return isAttacked;
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();

            attackService.Attacked -= OnAttacked;
        }


        private void OnAttacked(IGameplayEntity attacker, IGameplayEntity target, float damage)
        {
            IGameplayEntity attackTarget = null;

            if (config.AttackTarget == AbilityTarget.Self)
            {
                attackTarget = context.Owner;
            }
            else if (config.AttackTarget == AbilityTarget.Parent)
            {
                attackTarget = context.Owner.Parent;
            }

            if (!isAttacked && attackTarget != null && target.Id == attackTarget.Id)
            {
                cts?.Cancel();
                cts?.Dispose();

                cts = new CancellationTokenSource();

                SetAttacked(cts.Token).Forget();
            }
        }

        private async UniTask SetAttacked(CancellationToken cancellationToken)
        {
            isAttacked = true;

            await UniTaskHelper.DelayWithGameplayTimeScale(config.AttackedStateDuration, timeScaleHolder, cancellationToken);

            isAttacked = false;
        }
    }
}