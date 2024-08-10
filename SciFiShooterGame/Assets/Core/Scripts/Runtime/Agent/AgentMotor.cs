using System;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    public class AgentMotor : MonoBehaviour
    {
        [SerializeField] private Transform _aimMouse;
        
        private Agent _agent;     
        private float _verticalVelocity;
        private Vector3 _movementDirection;
        private Vector3 _lookingDirection;

        private const float _gravityScale = 9.81f;

        private void Awake()
        {
            _agent = GetComponent<Agent>();
        }

        private void Update()
        {
            MovementBehavior();    
            AimInputTowards();
            AnimatorControllers();
        }

        private void AimInputTowards()
        {
            Ray ray = _agent.FindMainCamera().ScreenPointToRay(_agent.AgentMovement.InputReader.AimInputValue);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _agent.AgentMovement.AimLayerMask))
            {
                _lookingDirection = hitInfo.point - transform.position;
                _lookingDirection.y = 0f;
                _lookingDirection.Normalize();

                transform.forward = _lookingDirection;

                _aimMouse.position = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
            }
        }

        private void MovementBehavior()
        {
            _movementDirection = new Vector3(_agent.AgentMovement.InputReader.MovementValue.x, 0,
                _agent.AgentMovement.InputReader.MovementValue.y);
            ApplyGravity();

            if (_movementDirection.magnitude > 0)
            {
                _agent.CharacterController.Move(_movementDirection *
                                                (_agent.AgentMovement.WalkSpeed * Time.deltaTime));
            }
        }

        private void ApplyGravity()
        {
            if (!_agent.CharacterController.isGrounded)
            {
                _verticalVelocity -= _gravityScale * Time.deltaTime;
                _movementDirection.y = _verticalVelocity;
            }
            else _verticalVelocity = -.5f;
        }

        private void AnimatorControllers()
        {
            float xVelocity = Vector3.Dot(_movementDirection.normalized, transform.right);
            float zVelocity = Vector3.Dot(_movementDirection.normalized, transform.forward);
            
            _agent.AgentAnimator.Animator.SetFloat(_agent.AgentAnimator.XVelocity, xVelocity,
                _agent.AgentAnimator.DampTime, Time.deltaTime );
            _agent.AgentAnimator.Animator.SetFloat(_agent.AgentAnimator.ZVelocity, zVelocity,
                _agent.AgentAnimator.DampTime, Time.deltaTime);
        }
    } 
}

