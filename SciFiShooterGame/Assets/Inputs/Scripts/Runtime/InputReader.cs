using System;
using GlobalInputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlobalInputs
{
    [CreateAssetMenu(menuName = "Global Inputs/New InputReader", fileName = "InputReader", order = 0)]
    public class InputReader : ScriptableObject, Controls.ICharacterActions
    {
        public Vector2 MovementValue { get; private set; }
        public Vector2 AimInputValue { get; private set; }
        public bool IsRunning { get; private set; }

        public event Action<bool> NotifyOnFire;

        
    
        private Controls _controls;
    
        public void InitializeControls()
        {
            _controls = new Controls();
            _controls.Character.SetCallbacks(this);        
            _controls.Character.Enable();
        }    
    
        public void DestroyControls() => _controls.Character.Disable();
    
    
    
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.canceled) return;   
        
            NotifyOnFire?.Invoke(context.performed);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementValue = context.ReadValue<Vector2>();
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            AimInputValue = context.ReadValue<Vector2>();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed) IsRunning = true;
            else if (context.canceled) IsRunning = false;   
        }
    }
}

