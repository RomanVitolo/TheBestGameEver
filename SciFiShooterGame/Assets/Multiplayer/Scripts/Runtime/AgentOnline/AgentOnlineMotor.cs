using System;
using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Movement;
using Core.Scripts.Runtime.Movement.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace Multiplayer.Runtime.AgentOnline
{
    public class AgentOnlineMotor : NetworkBehaviour
    {
        private Agent _agent;
        private Camera _mainCamera;
        
        private IMovementHandler _movementHandler;
        private IAimHandler _aimHandler;
        private IRotationHandler _rotationHandler;
        private IAnimationHandler _animationHandler;

        private LayerMask _aimLayerMask;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            
            _agent = GetComponent<Agent>();
            _agent.GetComponents();
            
            _aimLayerMask = _agent.AgentMovement.AimLayerMask;
            
            _mainCamera = _agent.AssignMainCamera();
            InitializeHandlers();
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner) return;
            
            
        }
        
        private void Update()
        {
            if (!IsOwner) return;
            
            Vector3 movementValue = _agent.AgentInputReader.MovementValue;
            
            _movementHandler.HandleMovement();
            _animationHandler.UpdateAnimation();
            
            RaycastHit mouseHitInfo = _agent.AgentAim.GetMouseHitInfo(_mainCamera, 
                _agent.AgentInputReader.AimInputValue, _aimLayerMask);
            
            _aimHandler.HandleAim(movementValue, mouseHitInfo);
            
            _rotationHandler.ApplyRotation(mouseHitInfo);
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