using Core.Scripts.Runtime.Agents.Interfaces;
using GlobalInputs;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    [RequireComponent(typeof(CharacterController))]      
    public class Agent : MonoBehaviour
    {   
        [field: SerializeField, Header("Inputs"), Space] public InputReader AgentInputReader { get; private set; }  
        [field: SerializeField, Header("Agent Movement"), Space] public AgentMovement AgentMovement { get; set; }
        [field: SerializeField, Header("Agent Animations"), Space] public AgentAnimatorSO AgentAnimator { get; private set; }
        public IAgentAim AgentAim { get; private set; }
        public CharacterController CharacterController { get; private set; }    
      
        public void GetComponents()
        {
            AgentInputReader.InitializeControls();
            CharacterController = GetComponent<CharacterController>();
            AgentAnimator.Animator = GetComponentInChildren<Animator>();
            AgentAim = GetComponent<IAgentAim>();
        }
        
        private void OnDestroy() => AgentInputReader.DestroyControls();        
                      
    }
}