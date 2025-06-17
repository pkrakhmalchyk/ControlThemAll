using System;
using ControllThemAll.Runtime.Utils;

namespace ControllThemAll.Runtime.Gameplay
{
    public class AttackService
    {
        public Action<IGameplayEntity, IGameplayEntity, float> Attacked;


        private readonly StatsService statsService;


        public AttackService(StatsService statsService)
        {
            this.statsService = statsService;
        }

        public void Attack(IGameplayEntity attacker, IGameplayEntity target, Damage damage)
        {
            StatsContainer targetStatsContainer = statsService.GetStats(target.Id);

            if (targetStatsContainer == null)
            {
                return;
            }

            if (!targetStatsContainer.TryGetStat(RuntimeConstants.Stats.Health, out IStat<float> healthStat))
            {
                return;
            }

            float floatDamage = CalculateFloatDamage(attacker, target, damage);

            healthStat.ChangeValue(healthStat.Value - floatDamage);
            Attacked?.Invoke(attacker, target, floatDamage);
        }


        private float CalculateFloatDamage(IGameplayEntity attacker, IGameplayEntity target, Damage damage)
        {
            if (damage.FloatDamage != 0)
            {
                return damage.FloatDamage;
            }
            else if (damage.PercentDamage != 0)
            {
                return CalculateFloatDamageFromPercent(target, damage);
            }

            return 0;
        }

        private float CalculateFloatDamageFromPercent(IGameplayEntity target, Damage damage)
        {
            StatsContainer targetStatsContainer = statsService.GetStats(target.Id);

            if (targetStatsContainer == null)
            {
                return 0;
            }

            if (!targetStatsContainer.TryGetStat(RuntimeConstants.Stats.Health, out IStat<float> healthStat))
            {
                return 0;
            }

            return healthStat.Value * damage.PercentDamage;
        }
    }
}