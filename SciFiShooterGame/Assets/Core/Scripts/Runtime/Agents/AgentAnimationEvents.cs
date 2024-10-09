using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public class AgentAnimationEvents : MonoBehaviour
    {
        private AgentWeaponMotor _agentWeaponMotor;

        private void Start() => _agentWeaponMotor = GetComponentInChildren<AgentWeaponMotor>(); 
       
        public void ReloadIsOver()
        {
            _agentWeaponMotor.MaximizeRigWeight();
            _agentWeaponMotor.CurrentWeapon().WeaponDataConfiguration.RefillAmmo();      
        }

        public void ReturnRig()
        {
            _agentWeaponMotor.MaximizeRigWeight();
            _agentWeaponMotor.MaximizeLeftHandWeight();    
        }

        public void WeaponGrabIsOveR()=> _agentWeaponMotor.SetBusyGrabbingWeaponTo(false);      
    }
}

