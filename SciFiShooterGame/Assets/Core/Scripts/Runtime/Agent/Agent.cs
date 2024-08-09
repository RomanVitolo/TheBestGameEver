using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    public class Agent : MonoBehaviour
    {
        [field: SerializeField] public AgentMovement AgentMovement { get; set; }
        public CharacterController CharacterController { get; private set; }

        private void Awake()
        {
            AgentMovement.InputReader.InitializeControls();
            CharacterController = GetComponent<CharacterController>();
        }

        private void OnDestroy()
        {
            AgentMovement.InputReader.DestroyControls();
        }
    }
}