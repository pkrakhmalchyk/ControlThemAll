using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    public class MouseTouchInput : ITouchInput
    {
        public bool IsActive => Input.GetMouseButton(0);
        public Vector3 Position => Input.mousePosition;
    }
}