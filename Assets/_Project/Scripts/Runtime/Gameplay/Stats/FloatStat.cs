using System;

namespace ControllThemAll.Runtime.Gameplay
{
    public class FloatStat : IStat<float>
    {
        public Action<IStat<float>, float, float> ValueChanged { get; set; }


        private float maxValue;
        private float value;


        public float MaxValue => maxValue;
        public float Value => value;


        public FloatStat(float maxValue, float value)
        {
            this.maxValue = maxValue;
            this.value = value;
        }

        public void ChangeValue(float newValue)
        {
            float oldValue = value;
            value = Math.Clamp(newValue, 0, maxValue);

            if (value != oldValue)
            {
                ValueChanged?.Invoke(this, oldValue, value);
            }
        }
    }
}