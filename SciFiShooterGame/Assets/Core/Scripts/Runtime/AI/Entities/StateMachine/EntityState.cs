using UnityEngine;
using UnityEngine.AI;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class EntityState
    {
        protected Entity entity;
        protected EntityStateMachine entityStateMachine;
        
        protected string animBoolName;
        protected float stateTimer;
        protected bool triggerCalled;

        public EntityState(Entity entity, EntityStateMachine entityStateMachine, string animBoolName)
        {
            this.entity = entity;
            this.entityStateMachine = entityStateMachine;
            this.animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            entity.Animator.SetBool(animBoolName, true);
            triggerCalled = false;
        }

        public virtual void Update()
        {
            stateTimer -= Time.deltaTime;
        }

        public virtual void Exit()
        {
            entity.Animator.SetBool(animBoolName, false);
        }

        public void AnimationTrigger() => triggerCalled = true;
        
        protected Vector3 GetNextPathPoint()
        {
            NavMeshAgent agent = entity.AIAgent;
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

