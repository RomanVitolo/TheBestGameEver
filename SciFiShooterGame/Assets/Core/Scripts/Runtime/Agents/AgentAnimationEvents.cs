using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public class AgentAnimationEvents : MonoBehaviour
    {
        private AgentWeaponMotor _agentWeaponMotor;
        private WeaponAnimations _weaponAnimations;

        private void Start()
        {
            _agentWeaponMotor = GetComponentInChildren<AgentWeaponMotor>(); 
            _weaponAnimations = GetComponentInChildren<WeaponAnimations>(); 
        } 
       
        public void ReloadIsOver()
        {
            _weaponAnimations.MaximizeRigWeight();
            _agentWeaponMotor.CurrentWeapon().WeaponDataConfiguration.RefillAmmo();    
            
            _agentWeaponMotor.SetWeaponReady(true);
        }

        public void ReturnRig()
        {
            _weaponAnimations.MaximizeRigWeight();
            _weaponAnimations.MaximizeLeftHandWeight();    
        }

        public void WeaponEquippingIsOveR()
        {
            _agentWeaponMotor.SetWeaponReady(true);
        }

    }
}

