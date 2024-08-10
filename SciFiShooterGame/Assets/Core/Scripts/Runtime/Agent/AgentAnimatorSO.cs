using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    [CreateAssetMenu(menuName = "Core/Agent Animation Values", fileName = "AgentAnimator")]
    public class AgentAnimatorSO : ScriptableObject
    {
        [field: SerializeField] public float DampTime { get; private set; }       
        public Animator Animator { get; set; }                           
        public int XVelocity { get; private set; } = Animator.StringToHash("xVelocity");
        public int ZVelocity { get; private set; } = Animator.StringToHash("zVelocity");
        public int IsRunning { get; private set; } = Animator.StringToHash("isRunning");
    }
}