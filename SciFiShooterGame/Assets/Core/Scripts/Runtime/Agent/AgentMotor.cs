using System;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    [RequireComponent(typeof(Agent))]
    public class AgentMotor : MonoBehaviour
    {   
        private Agent _agent;
        private AgentAim _agentAim;

        #region Aim&MovementFields
        
        private Camera _mainCamera;

        private LayerMask _aimLayerMask;   
       
        private Vector3 _lookingDirection; 
        private Vector3 _movementDirection;
        
        private float _verticalVelocity;
        private float _speed;
        private float _walkSpeed;
        private float _runSpeed;
        private float _turnSpeed;                                       
        
        private const float _gravityScale = 9.81f; 
        
        private void AssignAgentAimFields()
        {    
            _aimLayerMask = _agent.AgentMovement.AimLayerMask;  
        }

        private void AssignAgentMovementFields()
        {     
            _walkSpeed = _agent.AgentMovement.WalkSpeed;
            _runSpeed = _agent.AgentMovement.RunSpeed;   
            _turnSpeed = _agent.AgentMovement.TurnSpeed;   
            _speed = _walkSpeed;
        }

        #endregion        
        
        #region AnimationField

        private Animator _agentAnimator;
        private int _agentXVelocityHash;
        private int _agentZVelocityHash;    
        private int _agentIsRunningHash;
        private float _agentDampTime;
        
        private void AssignAnimationAgentFields()
        {
            _agentAnimator = _agent.AgentAnimator.Animator;
            _agentXVelocityHash = _agent.AgentAnimator.XVelocity;
            _agentZVelocityHash = _agent.AgentAnimator.ZVelocity;
            _agentIsRunningHash = _agent.AgentAnimator.IsRunning;      
            _agentDampTime = _agent.AgentAnimator.DampTime;
        }            
        
        #endregion
        
        #region UnityMethods

        private void Awake()
        {
            _agent = GetComponent<Agent>();
            _agentAim = GetComponent<AgentAim>();
            _agent.GetComponents();
        }

        private void OnEnable()
        {
            _mainCamera = _agent.AssignMainCamera();
            AssignAgentMovementFields(); 
            AssignAgentAimFields(); 
            AssignAnimationAgentFields();
        }     

        private void Update()
        {
            MovementBehavior();  
            _agentAim.UpdateAgentCameraPosition(_agentAim.GetMouseHitInfo(_mainCamera, 
                _agent.AgentInputReader.AimInputValue, _aimLayerMask).point, _agent.AgentInputReader.MovementValue);
            _agentAim.UpdateAgentAimPosition(_agentAim.GetMouseHitInfo(_mainCamera, 
                _agent.AgentInputReader.AimInputValue, _aimLayerMask).point);
            ApplyRotation();
            AnimatorControllers();
        }

        #endregion

        #region AimBehavior    
        private void ApplyRotation()
        {     
            _lookingDirection = _agentAim.GetMouseHitInfo(_mainCamera, _agent.AgentInputReader.AimInputValue
                ,_aimLayerMask).point - transform.position;
            _lookingDirection.y = 0f;
            _lookingDirection.Normalize();

            Quaternion desiredRotation = Quaternion.LookRotation(_lookingDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, _turnSpeed * Time.deltaTime); 
        }         
        #endregion

        #region MovementBehavior   
        private void MovementBehavior()
        {
            _movementDirection = new Vector3(_agent.AgentInputReader.MovementValue.x, 0,
                _agent.AgentInputReader.MovementValue.y);
            ApplyGravity();

            _speed = _agent.AgentInputReader.IsRunning ? _runSpeed : _walkSpeed;      
            
            if (_movementDirection.magnitude > 0) 
                _agent.CharacterController.Move(_movementDirection *
                                                (_speed * Time.deltaTime));
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
        #endregion

        #region AnimationsBehavior

        private void AnimatorControllers()
        {
            float xVelocity = Vector3.Dot(_movementDirection.normalized, transform.right);
            float zVelocity = Vector3.Dot(_movementDirection.normalized, transform.forward);
            
            _agentAnimator.SetFloat(_agentXVelocityHash, xVelocity, _agentDampTime, Time.deltaTime );
            _agentAnimator.SetFloat(_agentZVelocityHash, zVelocity, _agentDampTime, Time.deltaTime);

            bool playRunAnimation = _agent.AgentInputReader.IsRunning && _movementDirection.magnitude > 0;
            _agentAnimator.SetBool(_agentIsRunningHash, playRunAnimation);                  
        }

        #endregion    
    } 
}

