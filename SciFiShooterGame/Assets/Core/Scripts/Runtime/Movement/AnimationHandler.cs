using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Movement.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Movement
{
    public class AnimationHandler : IAnimationHandler
    {
        private readonly Agent _agent;
        private readonly Animator _animator;
        private readonly int _xVelocityHash;
        private readonly int _zVelocityHash;
        private readonly int _isRunningHash;
        private readonly float _dampTime;

        public AnimationHandler(Agent agent)
        {
            _agent = agent;
            _animator = _agent.AgentAnimator.Animator;
            _xVelocityHash = _agent.AgentAnimator.XVelocity;
            _zVelocityHash = _agent.AgentAnimator.ZVelocity;
            _isRunningHash = _agent.AgentAnimator.IsRunning;
            _dampTime = _agent.AgentAnimator.DampTime;
        }

        public void UpdateAnimation()
        {
            Vector3 movementDirection = new Vector3(_agent.AgentInputReader.MovementValue.x, 0, _agent.AgentInputReader.MovementValue.y);
            float xVelocity = Vector3.Dot(movementDirection.normalized, _agent.transform.right);
            float zVelocity = Vector3.Dot(movementDirection.normalized, _agent.transform.forward);

            _animator.SetFloat(_xVelocityHash, xVelocity, _dampTime, Time.deltaTime);
            _animator.SetFloat(_zVelocityHash, zVelocity, _dampTime, Time.deltaTime);

            bool shouldPlayRunAnimation = _agent.AgentInputReader.IsRunning && movementDirection.magnitude > 0;
            _animator.SetBool(_isRunningHash, shouldPlayRunAnimation);
        }
    }
}