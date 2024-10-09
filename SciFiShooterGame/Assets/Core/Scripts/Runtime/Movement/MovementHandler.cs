using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Movement.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Movement
{
    public class MovementHandler : IMovementHandler
    {
        private readonly Agent _agent;
        private float _verticalVelocity;
        private float _speed;

        private readonly float _walkSpeed;
        private readonly float _runSpeed;
        private readonly float _gravityScale = 9.81f;

        public MovementHandler(Agent agent)
        {
            _agent = agent;
            _walkSpeed = _agent.AgentMovement.WalkSpeed;
            _runSpeed = _agent.AgentMovement.RunSpeed;
        }

        public void HandleMovement()
        {
            Vector3 movementDirection = new Vector3(_agent.AgentInputReader.MovementValue.x, 0, _agent.AgentInputReader.MovementValue.y);
            ApplyGravity(ref movementDirection);

            _speed = _agent.AgentInputReader.IsRunning ? _runSpeed : _walkSpeed;

            if (movementDirection.magnitude > 0)
            {
                _agent.CharacterController.Move(movementDirection * (_speed * Time.deltaTime));
            }
        }

        private void ApplyGravity(ref Vector3 movementDirection)
        {
            if (!_agent.CharacterController.isGrounded)
            {
                _verticalVelocity -= _gravityScale * Time.deltaTime;
                movementDirection.y = _verticalVelocity;
            }
            else
            {
                _verticalVelocity = -0.5f;
            }
        }
    }
}