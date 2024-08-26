using GlobalInputs;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    [CreateAssetMenu(menuName = "Core/Agent Movement", fileName = "AgentMovement")]
    public class AgentMovement : ScriptableObject
    {               
        [field: SerializeField] public LayerMask AimLayerMask { get; private set; }
        [field: SerializeField] public float WalkSpeed { get; set; } 
        [field: SerializeField] public float RunSpeed { get; set; } 
        [field: SerializeField] public float TurnSpeed { get; set; } 
    }
}


