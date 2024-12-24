using Core.Scripts.Runtime.AI.Entities;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.StateMachine
{
    public class IdleState_Melee : EntityState
    {
        private Entity_Melee entity;
        
        public IdleState_Melee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) :
            base(entity, entityStateMachine, animBoolName)
        {
            this.entity = entity as Entity_Melee;
        }

        public override void Enter()
        {
            base.Enter();

            stateTimer = entity.IdleTime;
        }

        public override void Update()
        {
            base.Update();
            
            if(stateTimer < 0)
                entityStateMachine.ChangeState(entity.MoveState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}