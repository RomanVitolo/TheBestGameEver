using Core.Scripts.Runtime.Movement;
using Core.Scripts.Runtime.Movement.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    [RequireComponent(typeof(Agent))]
    public class AgentMotor : MonoBehaviour
    {   
        private Agent _agent;
        private Camera _mainCamera;
        
        private IMovementHandler _movementHandler;
        private IAimHandler _aimHandler;
        private IRotationHandler _rotationHandler;
        private IAnimationHandler _animationHandler;

        private LayerMask _aimLayerMask;

        private void Awake()
        {
            _agent = GetComponent<Agent>();
            _agent.GetComponents();
        }

        private void OnEnable()
        {
            _mainCamera = _agent.AssignMainCamera();
            _aimLayerMask = _agent.AgentMovement.AimLayerMask;
            InitializeHandlers();
        }

        private void Update()
        {
            _movementHandler.HandleMovement();
            
            // Get necessary parameters for aim
            Vector3 movementValue = _agent.AgentInputReader.MovementValue;
            RaycastHit mouseHitInfo = _agent.AgentAim.GetMouseHitInfo(_mainCamera, _agent.AgentInputReader.AimInputValue, _aimLayerMask);

            // Pass parameters to aim handler
            _aimHandler.HandleAim(movementValue, mouseHitInfo);
            
            _rotationHandler.ApplyRotation(mouseHitInfo);
            _animationHandler.UpdateAnimation();
        }

        private void InitializeHandlers()
        {
            _movementHandler = new MovementHandler(_agent);
            _aimHandler = new AimHandler(_agent);
            _rotationHandler = new RotationHandler(_agent, _mainCamera, _aimLayerMask);
            _animationHandler = new AnimationHandler(_agent);
        }
    } 
}

