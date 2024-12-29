using Core.Scripts.Runtime.AI.Entities;
using UnityEngine;
using UnityEngine.AI;

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
            entity.MeleeAgent.SetDestination(destination);
        }

        public override void Update()
        {
            base.Update();

            entity.transform.rotation = entity.FaceTarget(GetNextPathPoint());
            
            if(entity.MeleeAgent.remainingDistance <= entity.MeleeAgent.stoppingDistance + .05f)
                entityStateMachine.ChangeState(entity.IdleState);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public Vector3 GetNextPathPoint()
        {
            NavMeshAgent agent = entity.MeleeAgent;
            NavMeshPath path = agent.path;

            if (path.corners.Length < 2)
                return agent.destination;

            for (int i = 0; i < path.corners.Length; i++)
            {
                if(Vector3.Distance(agent.transform.position, path.corners[i]) < 1)
                    return path.corners[i + 1];
            }

            return agent.destination;
        }
    }
}