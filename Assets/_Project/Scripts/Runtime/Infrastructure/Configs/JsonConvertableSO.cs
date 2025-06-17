using UnityEngine;

namespace ControllThemAll.Runtime.Infrastructure
{
    public abstract class JsonConvertableSO : ScriptableObject
    {
        public abstract string ToJson();
    }
}