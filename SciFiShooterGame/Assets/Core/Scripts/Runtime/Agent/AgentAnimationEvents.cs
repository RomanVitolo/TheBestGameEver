using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    public class AgentAnimationEvents : MonoBehaviour
    {
        private AgentWeaponMotor _agentWeaponMotor;

        private void Start() => _agentWeaponMotor = GetComponentInChildren<AgentWeaponMotor>(); 
       
        public void ReloadIsOver()
        {
            _agentWeaponMotor.MaximizeRigWeight();
        
            //refill-bullets
        }

        public void ReturnRig()
        {
            _agentWeaponMotor.MaximizeRigWeight();
            _agentWeaponMotor.MaximizeLeftHandWeight();    
        }

        public void WeaponGrabIsOveR()=> _agentWeaponMotor.SetBusyGrabbingWeaponTo(false);      
    }
}

