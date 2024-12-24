using Core.Scripts.Runtime.AI.Entities;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.StateMachine
{
    public class EntityState
    {
        protected Entity entity;
        protected EntityStateMachine entityStateMachine;
        
        protected string animBoolName;
        protected float stateTimer;

        public EntityState(Entity entity, EntityStateMachine entityStateMachine, string animBoolName)
        {
            this.entity = entity;
            this.entityStateMachine = entityStateMachine;
            this.animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            Debug.Log("I Enter " + animBoolName + " state!");
        }

        public virtual void Update()
        {
            Debug.Log("I Update " + animBoolName + " state!");
            stateTimer -= Time.deltaTime;
        }

        public virtual void Exit()
        {
            
        }
    }
}

