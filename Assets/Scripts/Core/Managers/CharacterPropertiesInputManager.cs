using System;
using UnityEngine;

namespace Assets.Scripts.Core.Managers
{
    public static class InputManager
    {
        public static event Action OnMouseClick;
        public static event Action OnKeyAlpha1;
        public static event Action OnKeyAlpha2;
        public static event Action OnKeyAlpha3;

        public static void UpdateInputAttack()
        {
            if (Input.GetKeyUp(KeyCode.Mouse0)) OnMouseClick?.Invoke();
            if (Input.GetKeyUp(KeyCode.Alpha1)) OnKeyAlpha1?.Invoke();
            if (Input.GetKeyUp(KeyCode.Alpha2)) OnKeyAlpha2?.Invoke();
            if (Input.GetKeyUp(KeyCode.Alpha3)) OnKeyAlpha3?.Invoke();
        }
    }
}
