using GlobalInputs;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    [RequireComponent(typeof(CharacterController))]      
    public class Agent : MonoBehaviour
    {   
        [field: SerializeField, Header("Inputs"), Space] public InputReader AgentInputReader { get; private set; }  
        [field: SerializeField, Header("Agent Movement"), Space] public AgentMovement AgentMovement { get; set; }
        [field: SerializeField, Header("Agent Animations"), Space] public AgentAnimatorSO AgentAnimator { get; private set; }
        [field: SerializeField, Header("Agent Aim"), Space] public AgentAim AgentAim { get; private set; }
        public CharacterController CharacterController { get; private set; }    
        private Camera MainCamera;                  
      
        public void GetComponents()
        {
            AgentInputReader.InitializeControls();
            CharacterController = GetComponent<CharacterController>();
            AgentAnimator.Animator = GetComponentInChildren<Animator>();
            AgentAim = GetComponent<AgentAim>();
        }
        
        private void OnDestroy() => AgentInputReader.DestroyControls();         

        public Camera AssignMainCamera()
        {
            MainCamera = Camera.main;
            return MainCamera;
        }                
    }
}