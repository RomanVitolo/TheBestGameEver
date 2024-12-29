using Core.Scripts.Runtime.AI.StateMachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Core.Scripts.Runtime.AI.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [field: SerializeField] public float IdleTime { get; set; }
        [field: SerializeField] public float MoveSpeed { get; set; }
        [field: SerializeField] public float TurnSpeed { get; set; }
        [field: SerializeField] public Animator Animator { get; private set; } 
        [field: SerializeField] protected NavMeshAgent AIAgent { get; private set; }
        
        [SerializeField] protected Transform[] _patrolPoints;
        protected EntityStateMachine StateMachine { get; private set; }
        
        private int currentPatrolIndex;
        
        protected virtual void Awake()
        {
            StateMachine = new EntityStateMachine();
            if (AIAgent == null)
                AIAgent = GetComponent<NavMeshAgent>();
        }
    
        protected virtual void Start()
        {
            
        }
    
        protected virtual void Update()
        {
       
        }

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
            
            float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y, TurnSpeed * Time.deltaTime);
            
            return Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
        }
    }
}
