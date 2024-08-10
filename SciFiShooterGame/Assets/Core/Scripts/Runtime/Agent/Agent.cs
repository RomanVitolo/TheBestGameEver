using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    [RequireComponent(typeof(CharacterController))]      
    public class Agent : MonoBehaviour
    {
        [field: SerializeField] public AgentMovement AgentMovement { get; set; }
        [field: SerializeField] public AgentAnimatorSO AgentAnimator { get; private set; }
        [field: SerializeField] public Transform AimPoint { get; private set; }
        public CharacterController CharacterController { get; private set; }

        private Camera MainCamera;                  
      
        public void GetComponents()
        {
            AgentMovement.InputReader.InitializeControls();
            CharacterController = GetComponent<CharacterController>();
            AgentAnimator.Animator = GetComponentInChildren<Animator>();
        }
        
        private void OnDestroy()
        {
            AgentMovement.InputReader.DestroyControls();
        }      

        public Camera FindMainCamera()
        {
            MainCamera = Camera.main;
            return MainCamera;
        }

      
    }
}