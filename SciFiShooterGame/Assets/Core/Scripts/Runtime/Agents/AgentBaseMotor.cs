using Core.Scripts.Runtime.Movement.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public abstract class AgentBaseMotor : MonoBehaviour 
    {
        protected Agent _agent;
        protected Camera _mainCamera;
        
        protected IMovementHandler _movementHandler;
        protected IAimHandler _aimHandler;
        protected IRotationHandler _rotationHandler;
        protected IAnimationHandler _animationHandler;

        protected LayerMask _aimLayerMask;
    }
}