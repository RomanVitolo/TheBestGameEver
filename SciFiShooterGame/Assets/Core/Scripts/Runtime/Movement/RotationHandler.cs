using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Movement.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Movement
{
    public class RotationHandler : IRotationHandler
    {
        private readonly Agent _agent;
        private readonly Camera _camera;
        private readonly LayerMask _aimLayerMask;
        private readonly float _turnSpeed;

        public RotationHandler(Agent agent, Camera camera, LayerMask aimLayerMask)
        {
            _agent = agent;
            _camera = camera;
            _aimLayerMask = aimLayerMask;
            _turnSpeed = _agent.AgentMovement.TurnSpeed;
        }

        public void ApplyRotation(RaycastHit mouseHitInfo)
        {
            Vector3 lookDirection = mouseHitInfo.point - _agent.transform.position;
            lookDirection.y = 0f;
            lookDirection.Normalize();

            Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
            _agent.transform.rotation = Quaternion.Slerp(_agent.transform.rotation, desiredRotation, _turnSpeed * Time.deltaTime);
        }
    }
}