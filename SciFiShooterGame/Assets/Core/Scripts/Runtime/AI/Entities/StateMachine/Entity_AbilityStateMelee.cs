using Core.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class Entity_AbilityStateMelee : EntityState
    {
        private static readonly int RecoveryIndex = Animator.StringToHash("RecoveryIndex");
        private Entity_Melee _entityMelee;
        private Vector3 _moveDirection;
        private float _moveSpeed;
        
        private const float MAX_MOVEMENT_DISTANCE = 5f;
        public Entity_AbilityStateMelee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) : base(entity, entityStateMachine, animBoolName)
        {
            _entityMelee = entity as Entity_Melee;
        }

        public override void Enter()
        {
            base.Enter();

            _moveSpeed = _entityMelee.ChaseSpeed;
            _moveDirection = _entityMelee.transform.position + (_entityMelee.transform.forward * MAX_MOVEMENT_DISTANCE);
        }

        public override void Update()
        {
            base.Update();
            
            if (_entityMelee.ManualRotationActive())
            {
                _entityMelee.FaceTarget(_entityMelee.Target.position);
                _moveDirection = _entityMelee.transform.position + (_entityMelee.transform.forward * MAX_MOVEMENT_DISTANCE);
            }
            
            if (_entityMelee.ManualMovementActive())
                _entityMelee.transform.position = Vector3.MoveTowards(_entityMelee.transform.position, _moveDirection,
                    _entityMelee.ChaseSpeed * Time.deltaTime);
            
            if(triggerCalled) 
                entityStateMachine.ChangeState(_entityMelee.RecoveryState);
        }

        public override void Exit()
        {
            base.Exit();
            _entityMelee.ChaseSpeed = _moveSpeed;
            _entityMelee.Animator.SetFloat(RecoveryIndex, 0);
        }

        public override void AbilityTrigger()
        {
            base.AbilityTrigger();

            var newThrowWeapon = GlobalPoolContainer.Instance.WeaponThrow.GetObject();
            newThrowWeapon.transform.position = _entityMelee.WeaponThrowStartPoint.position;
            
            newThrowWeapon.gameObject.GetComponent<Entity_WeaponThrow>()
                .WeaponThrowSetup(_entityMelee.WeaponThrowSpeed, _entityMelee.Target, _entityMelee.WeaponThrowAimTimer);
        }
    }
}