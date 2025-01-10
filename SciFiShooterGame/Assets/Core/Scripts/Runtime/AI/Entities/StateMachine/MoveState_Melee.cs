using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class MoveState_Melee : EntityState
    {
        private readonly Entity_Melee _entity;
        private Vector3 destination;
        
        public MoveState_Melee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) : 
            base(entity, entityStateMachine, animBoolName)
        {
            this._entity = entity as Entity_Melee;
        }
        public override void Enter()
        {
            base.Enter();

            _entity.MeleeAgent.speed = _entity.AttackData.MoveSpeed;

            destination = _entity.GetPatrolDestination();
            _entity.MeleeAgent.SetDestination(destination);
        }

        public override void Update()
        {
            base.Update();

            if (_entity.TargetInAggressionRange())
            {
                entityStateMachine.ChangeState(_entity.RecoveryState);
                return;
            }

            _entity.transform.rotation = _entity.FaceTarget(GetNextPathPoint());
            
            if(_entity.MeleeAgent.remainingDistance <= _entity.MeleeAgent.stoppingDistance + .05f)
                entityStateMachine.ChangeState(_entity.IdleState);
        }

        public override void Exit()
        {
            base.Exit();
        }

        
    }
}