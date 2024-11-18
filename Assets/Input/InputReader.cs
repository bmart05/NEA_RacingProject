using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using static PlayerControls;

namespace Input
{
    [CreateAssetMenu(menuName = "InputReader", fileName = "InputReader", order = 0)]
    public class InputReader : ScriptableObject, IPlayerActions
    {
        private PlayerControls controls;

        public Action<float> TurnEvent;
        public Action<float> AccelerateEvent;
        public Action<bool> FireEvent;
        
        private void OnEnable()
        {
            if (controls == null)
            {
                controls = new PlayerControls();
                controls.Player.SetCallbacks(this);
            }

            controls.Player.Enable();
        }

        private void OnDisable()
        {
            controls.Player.Disable(); //frees memory
        }

        public void OnTurn(InputAction.CallbackContext context)
        {
            TurnEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnAccelerate(InputAction.CallbackContext context)
        {
            AccelerateEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                FireEvent?.Invoke(true);
            }
            if (context.canceled)
            {
                FireEvent?.Invoke(false);
            }
        }
    }
}