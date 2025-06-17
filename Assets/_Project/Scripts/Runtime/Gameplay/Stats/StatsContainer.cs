using System;
using System.Collections.Generic;
using System.Linq;

namespace ControllThemAll.Runtime.Gameplay
{
    public class StatsContainer
    {
        public Action<StatsContainer, string, float, float> StatChanged;

        private readonly Dictionary<string, IStat<float>> floatStats;


        public StatsContainer()
        {
            floatStats = new Dictionary<string, IStat<float>>();
        }

        public void AddStat(string key, IStat<float> stat)
        {
            floatStats[key] = stat;

            stat.ValueChanged += OnStatValueChanged;
        }

        public bool TryGetStat(string key, out IStat<float> stat)
        {
            if (floatStats.TryGetValue(key, out IStat<float> floatStat))
            {
                stat = floatStat;

                return true;
            }

            stat = null;

            return false;
        }


        private void OnStatValueChanged(IStat<float> changedStat, float previousValue, float newValue)
        {
            string statName = floatStats.FirstOrDefault(stat => stat.Value == changedStat).Key;

            StatChanged?.Invoke(this, statName, previousValue, newValue);
        }
    }
}