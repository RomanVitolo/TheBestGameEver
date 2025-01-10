using System;
using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.AI.Entities.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Core.Scripts.Runtime.AI.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [field: SerializeField] public AttackData AttackData { get; set; }
        [field: SerializeField] public float IdleTime { get; set; }
        //[field: SerializeField] public float MoveSpeed { get; set; }
        [field: SerializeField] public float TurnSpeed { get; set; }
        [field: SerializeField] public float ChaseSpeed { get; set; }
        [field: SerializeField] public float AggressionRange { get; set; }
        //[field: SerializeField] public float AttackRange { get; set; }
       // [field: SerializeField] public float AttackMoveSpeed { get; set; }
        [field: SerializeField] public Transform Target { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; } 
        [field: SerializeField] public NavMeshAgent AIAgent { get; private set; }
        
        [SerializeField] protected Transform[] _patrolPoints;
        protected EntityStateMachine StateMachine { get; private set; }
        
        private int currentPatrolIndex;
        private bool manualMovement;
        private bool manualRotation;
        
        protected virtual void Awake()
        {
            StateMachine = new EntityStateMachine();
            if (AIAgent == null)
                AIAgent = GetComponent<NavMeshAgent>();
            if (Target == null)
                Target = FindAnyObjectByType<Agent>().gameObject.transform;
        }
    
        protected virtual void Start(){}

        protected virtual void Update() { }


        public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

        public bool TargetInAggressionRange() => 
            Vector3.Distance(transform.position, Target.position) < AggressionRange;

        public bool TargetInAttackRange() => Vector3.Distance(transform.position, Target.position) < AttackData.AttackRange;
       
        public Vector3 GetPatrolDestination()
        {
            Vector3 destination = _patrolPoints[currentPatrolIndex].position;
            currentPatrolIndex++;
            
            if(currentPatrolIndex >= _patrolPoints.Length)
                currentPatrolIndex = 0;
            
            return destination;
        }

        public Quaternion FaceTarget(Vector3 target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            
            Vector3 currentEulerAngles = transform.eulerAngles;
            
            float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y,
                TurnSpeed * Time.deltaTime);
            
            return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
        }
        
        public void ActivateManualMovement(bool canManualMovement) => this.manualMovement = canManualMovement;
        public bool ManualMovementActive() => manualMovement;
        public bool ManualRotationActive() => manualRotation;
        
        public void ActivateManualRotation(bool canManualRotation) => this.manualRotation = canManualRotation;

        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, AggressionRange);
        }
    }
    
    [System.Serializable]
    public struct AttackData
    {
        public float AttackRange;
        public float AttackMoveSpeed;
        public float MoveSpeed;
        public float AttackIndex;
        [Range(1,2)] public float AnimationSpeed;
    }
}
