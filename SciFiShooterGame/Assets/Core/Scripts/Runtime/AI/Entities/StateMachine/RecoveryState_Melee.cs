namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class RecoveryState_Melee : EntityState
    {
        private readonly Entity_Melee _entity;
        
        public RecoveryState_Melee(Entity entity, EntityStateMachine entityStateMachine,
            string animBoolName) : base(entity, entityStateMachine, animBoolName)
        {
            _entity = entity as Entity_Melee;
        }

        public override void Enter()
        {
            base.Enter();
            
            _entity.MeleeAgent.isStopped = true;
        }

        public override void Update()
        {
            base.Update();
            
            _entity.transform.rotation = entity.FaceTarget(entity.Target.position);

            if (triggerCalled)
            {
                if(_entity.TargetInAttackRange())
                    entityStateMachine.ChangeState(_entity.AttackState);
                else
                    entityStateMachine.ChangeState(_entity.ChaseState);
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}