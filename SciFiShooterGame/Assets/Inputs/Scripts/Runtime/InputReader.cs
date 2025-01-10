using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GlobalInputs
{
    [CreateAssetMenu(menuName = "Global Inputs/New InputReader", fileName = "InputReader", order = 0)]
    public class InputReader : ScriptableObject, Controls.ICharacterActions, Controls.IWeaponActions
    {
        public Vector2 MovementValue { get; private set; } = Vector3.zero;
        public Vector2 AimInputValue { get; private set; }
        public bool IsRunning { get; private set; }
        public int WeaponSlotLocation { get; private set; }
        public bool CanShoot { get;  set; }
        
        public event Action NotifyWeaponSwitch;
        public event Action NotifyMainWeaponSwitch;
        public event Action NotifySecondaryWeaponSwitch;
        public event Action NotifyMeleeWeaponSwitch;
        public event Action NotifyThrowableWeaponSwitch;
        public event Action NotifyWeaponReload;
        public event Action NotifyWhenWeaponDropped;
        public event Action NotifyWhenWeaponFireModeChanged;
        
    
        private Controls _controls;
    
        public void InitializeControls()
        {
            _controls = new Controls();
            _controls.Character.SetCallbacks(this);     
            _controls.Weapon.SetCallbacks(this);
            _controls.Character.Enable();
            _controls.Weapon.Enable();
        }

        public void DestroyControls()
        {
            _controls.Character.Disable();
            _controls.Weapon.Disable();
        } 
    
        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed) CanShoot = true;
            else if (context.canceled) CanShoot = false;
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

        public void OnMainWeapon(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            WeaponSlotLocation = 0;
            NotifyMainWeaponSwitch?.Invoke();
        }

        public void OnSecondaryWeapon(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            WeaponSlotLocation = 1;
            NotifySecondaryWeaponSwitch?.Invoke();
        }

        public void OnMeleeWeapon(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            WeaponSlotLocation = 2;
            NotifyMeleeWeaponSwitch?.Invoke();
        }

        public void OnThrowableWeapon(InputAction.CallbackContext context)
        {
            if (!context.performed) return;   
            
            NotifyThrowableWeaponSwitch?.Invoke();
        }

        public void OnSwitchWeapons(InputAction.CallbackContext context)
        {
            if (!context.performed) return;   
            
            NotifyWeaponSwitch?.Invoke();
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (!context.performed) return;   
            
            NotifyWeaponReload?.Invoke();
        }

        public void OnPreciseShooting(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnDropWeapon(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            NotifyWhenWeaponDropped?.Invoke();
        }

        public void OnFireMode(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            NotifyWhenWeaponFireModeChanged?.Invoke();
        }
    }
}

