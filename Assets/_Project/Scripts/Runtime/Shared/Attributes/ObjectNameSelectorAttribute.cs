using System;
using UnityEngine;

namespace ControllThemAll.Runtime.Shared
{
    public class ObjectNameSelectorAttribute : PropertyAttribute
    {
        public Type Type;

        public ObjectNameSelectorAttribute(Type type)
        {
            this.Type = type;
        }
    }
}