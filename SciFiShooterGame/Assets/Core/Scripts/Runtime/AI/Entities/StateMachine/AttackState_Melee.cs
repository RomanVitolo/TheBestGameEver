using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class AttackState_Melee : EntityState
    {
        private static readonly int SlashAttackIndex = Animator.StringToHash("SlashAttackIndex");
        private static readonly int RecoveryIndex = Animator.StringToHash("RecoveryIndex");
        private static readonly int AttackAnimationSpeed = Animator.StringToHash("AttackAnimationSpeed");
        private static readonly int AttackIndex = Animator.StringToHash("AttackIndex");
        
        private readonly Entity_Melee _entity;
        private Vector3 attackDirection;

        private const float MAX_ATTACK_DISTANCE = 10f;
        
        public AttackState_Melee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) 
            : base(entity, entityStateMachine, animBoolName)
        {
            _entity = entity as Entity_Melee;
        }

        public override void Enter()
        {
            base.Enter();

            //_entity.PullWeapon();
            
            _entity.Animator.SetFloat(AttackAnimationSpeed, _entity.AttackData.AnimationSpeed);
            _entity.Animator.SetFloat(AttackIndex, _entity.AttackData.AttackIndex);
            _entity.Animator.SetFloat(SlashAttackIndex, Random.Range(0f, 5));

            _entity.MeleeAgent.isStopped = true;
            _entity.MeleeAgent.velocity = Vector3.zero;

            attackDirection = _entity.transform.position + (_entity.transform.forward * MAX_ATTACK_DISTANCE);
        }

        public override void Update()
        {
            base.Update();

            if (_entity.ManualRotationActive())
            {
                _entity.transform.rotation = _entity.FaceTarget(_entity.Target.position);
                attackDirection = _entity.transform.position + (_entity.transform.forward * MAX_ATTACK_DISTANCE);
            }
            
            if (_entity.ManualMovementActive())
                _entity.transform.position = Vector3.MoveTowards(_entity.transform.position, attackDirection,
                    _entity.AttackData.AttackMoveSpeed * Time.deltaTime);

            if (!triggerCalled) return;
            if(_entity.TargetInAttackRange())
                entityStateMachine.ChangeState(_entity.RecoveryState);
            else
                entityStateMachine.ChangeState(_entity.ChaseState);
        }

        public override void Exit()
        {
            base.Exit();

            SetupNextAttack();
        }

        private void SetupNextAttack()
        {
            int recoveryIndex = TargetIsClose() ? 1 : 0;
            _entity.Animator.SetFloat(RecoveryIndex, recoveryIndex);

            _entity.AttackData = UpdateAttackData();
        }
        private bool TargetIsClose() => Vector3.Distance(_entity.transform.position, _entity.Target.position) <= 1f;
        private AttackData UpdateAttackData()
        {
            List<AttackData> validAttacks = new List<AttackData>(_entity.AttackList);
           
            if(TargetIsClose())
                validAttacks.RemoveAll(parameter => parameter.AttackType == AttackType_Melee.ChargeAttack);
            
            int random = Random.Range(0, validAttacks.Count);
            return validAttacks[random];
        }
    }
}