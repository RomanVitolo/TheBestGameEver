using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Multiplayer.Runtime.AgentOnline
{
    public class AgentOnlineAnimationEvents : MonoBehaviour
    {
        private AgentOnlineWeaponMotor _agentWeaponMotor;
        private WeaponAnimations _weaponAnimations;

        [SerializeField] private bool _isOnline;

        private void Start()
        {
            _agentWeaponMotor = GetComponentInChildren<AgentOnlineWeaponMotor>(); 
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