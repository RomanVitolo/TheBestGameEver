using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    public class Agent : MonoBehaviour
    {
        [field: SerializeField] public AgentMovement AgentMovement { get; set; }
        [field: SerializeField] public AgentAnimatorSO AgentAnimator { get; private set; }
        public CharacterController CharacterController { get; private set; }
        
        public Camera MainCamera { get; private set; }

        private void Awake()
        {
            AgentMovement.InputReader.InitializeControls();
            CharacterController = GetComponent<CharacterController>();
            AgentAnimator.Animator = GetComponentInChildren<Animator>();
        }

        public Camera FindMainCamera()
        {
            MainCamera = Camera.main;
            return MainCamera;
        }

        private void OnDestroy()
        {
            AgentMovement.InputReader.DestroyControls();
        }
    }
}