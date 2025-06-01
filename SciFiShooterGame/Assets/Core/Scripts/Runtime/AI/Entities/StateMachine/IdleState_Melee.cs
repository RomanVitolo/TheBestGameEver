namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class IdleState_Melee : EntityState
    {
        private Entity_Melee _entity;
        
        public IdleState_Melee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) :
            base(entity, entityStateMachine, animBoolName)
        {
            this._entity = entity as Entity_Melee;
        }

        public override void Enter()
        {
            base.Enter();

            stateTimer = _entity.IdleTime;
        }

        public override void Update()
        {
            base.Update();
            
            if(stateTimer < 0)
                entityStateMachine.ChangeState(_entity.MoveState);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}