using Core.Scripts.Runtime.AI.Entities;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.StateMachine
{
    public class MoveState_Melee : EntityState
    {
        private Entity_Melee entity;
        private Vector3 destination;
        
        public MoveState_Melee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) : 
            base(entity, entityStateMachine, animBoolName)
        {
            this.entity = entity as Entity_Melee;
        }
        public override void Enter()
        {
            base.Enter();

            destination = entity.GetPatrolDestination();
        }

        public override void Update()
        {
            base.Update();

            entity.AIAgent.SetDestination(destination);
            
            if(entity.AIAgent.remainingDistance <= 1)
                entityStateMachine.ChangeState(entity.IdleState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}