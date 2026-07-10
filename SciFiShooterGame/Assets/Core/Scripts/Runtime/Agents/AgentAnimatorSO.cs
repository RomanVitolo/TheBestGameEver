using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    /// <summary>
    /// Shared, read-only animation tuning. The Animator itself lives on <see cref="Agent.Animator"/>: as an
    /// asset field it was overwritten by whichever agent spawned last, so every agent drove one Animator.
    /// </summary>
    [CreateAssetMenu(menuName = "Core/Agent Animation Values", fileName = "AgentAnimator")]
    public class AgentAnimatorSO : ScriptableObject
    {
        [field: SerializeField] public float DampTime { get; private set; }
        public int XVelocity { get; private set; } = Animator.StringToHash("xVelocity");
        public int ZVelocity { get; private set; } = Animator.StringToHash("zVelocity");
        public int IsRunning { get; private set; } = Animator.StringToHash("isRunning");  
    }
}