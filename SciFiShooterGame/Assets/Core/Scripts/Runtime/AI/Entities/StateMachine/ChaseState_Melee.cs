using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class ChaseState_Melee : EntityState
    {
        private readonly Entity_Melee _entity;
        private float lastTimeUpdatedDestination;
        
        public ChaseState_Melee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) : base(entity, entityStateMachine, animBoolName)
        {
            _entity = entity as Entity_Melee;
        }

        public override void Enter()
        {
            base.Enter();
            
            _entity.MeleeAgent.speed = _entity.ChaseSpeed;
            entity.AIAgent.isStopped = false;
        }

        public override void Update()
        {
            base.Update();
            
            if(entity.TargetInAttackRange()) 
                entityStateMachine.ChangeState(_entity.AttackState);
            
            entity.transform.rotation = entity.FaceTarget(GetNextPathPoint());

            if (UpdateDestination())
                _entity.MeleeAgent.destination = entity.Target.transform.position;
        }

        public override void Exit()
        {
            base.Exit();
        }

        private bool UpdateDestination()
        {
            if (!(Time.time > lastTimeUpdatedDestination + 0.25f)) return false;
            lastTimeUpdatedDestination = Time.time;
            return true;
        }
    }
}
