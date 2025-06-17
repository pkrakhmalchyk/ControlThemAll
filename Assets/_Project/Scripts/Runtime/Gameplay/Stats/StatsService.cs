using System;
using System.Collections.Generic;
using System.Linq;
using ControllThemAll.Runtime.Utils;

namespace ControllThemAll.Runtime.Gameplay
{
    public class StatsService
    {
        public Action<int, float, float> HealthStatChanged;

        private Dictionary<int, StatsContainer> entitiesStatsContainers;


        public StatsService()
        {
            entitiesStatsContainers = new Dictionary<int, StatsContainer>();
        }

        public void BindStats(int ownerId, StatsContainer statsContainer)
        {
            ClearStats(ownerId);
            entitiesStatsContainers.Add(ownerId, statsContainer);

            statsContainer.StatChanged += OnStatChanged;
        }

        public StatsContainer GetStats(int ownerId)
        {
            if (entitiesStatsContainers.TryGetValue(ownerId, out StatsContainer statsContainer))
            {
                return statsContainer;
            }

            return null;
        }

        public void ClearStats(int ownerId)
        {
            if (entitiesStatsContainers.TryGetValue(ownerId, out StatsContainer statsContainer))
            {
                statsContainer.StatChanged -= OnStatChanged;

                entitiesStatsContainers.Remove(ownerId);
            }
        }

        public void Dispose()
        {
            foreach (KeyValuePair<int, StatsContainer> entityStats in entitiesStatsContainers)
            {
                ClearStats(entityStats.Key);
            }
        }


        private void OnStatChanged(StatsContainer changedStatsContainer, string statName, float previousValue, float newValue)
        {
            int entityId = entitiesStatsContainers
                .FirstOrDefault(entityStatsContainer => entityStatsContainer.Value == changedStatsContainer).Key;

            if (statName == RuntimeConstants.Stats.Health)
            {
                HealthStatChanged?.Invoke(entityId, previousValue, newValue);
            }
        }
    }
}