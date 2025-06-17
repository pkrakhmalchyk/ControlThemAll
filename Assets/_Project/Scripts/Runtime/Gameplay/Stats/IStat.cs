using System;

namespace ControllThemAll.Runtime.Gameplay
{
    public interface IStat<T>
    {
        public Action<IStat<T>, T, T> ValueChanged { get; set; }
        public T Value { get; }
        public T MaxValue { get; }
        public void ChangeValue(T newValue);
    }
}