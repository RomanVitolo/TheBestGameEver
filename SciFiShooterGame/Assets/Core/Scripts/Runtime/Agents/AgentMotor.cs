using Core.Scripts.Runtime.Movement;
using Core.Scripts.Runtime.Movement.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    /// <summary>
    /// Owner-authoritative locomotion. Only the client that owns this agent simulates movement, rotation and
    /// aim; the resulting transform is replicated to everyone else by the NetworkTransform on the prefab,
    /// and the pose feeds the NetworkAnimator. Remote agents run no handlers at all.
    /// </summary>
    [RequireComponent(typeof(Agent))]
    public class AgentMotor : MonoBehaviour
    {
        private Agent _agent;

        private IMovementHandler _movementHandler;
        private IAimHandler _aimHandler;
        private IRotationHandler _rotationHandler;
        private IAnimationHandler _animationHandler;

        private LayerMask _aimLayerMask;
        private bool _handlersReady;

        private void Awake() => _agent = GetComponent<Agent>();

        private void Update()
        {
            if (!_agent.IsSpawned || !_agent.IsOwner || !_agent.Health.IsAlive) return;

            if (!_handlersReady)
                InitializeHandlers();

            Vector3 movementValue = _agent.AgentInputReader.MovementValue;

            _movementHandler.HandleMovement();
            _animationHandler.UpdateAnimation();

            RaycastHit mouseHitInfo = _agent.AgentAim.GetMouseHitInfo(_agent.AgentCamera,
                _agent.AgentInputReader.AimInputValue, _aimLayerMask);

            _aimHandler.HandleAim(movementValue, mouseHitInfo);

            _rotationHandler.ApplyRotation(mouseHitInfo);
        }

        private void InitializeHandlers()
        {
            _aimLayerMask = _agent.AgentMovement.AimLayerMask;

            _movementHandler = new MovementHandler(_agent);
            _aimHandler = new AimHandler(_agent);
            _rotationHandler = new RotationHandler(_agent, _agent.AgentCamera, _aimLayerMask);
            _animationHandler = new AnimationHandler(_agent);

            _handlersReady = true;
        }
    }
}
