using Core.Scripts.Runtime.AI.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Scripts.Runtime.AI.Entities
{
    public class Entity : MonoBehaviour
    {
        public float IdleTime;
        public float MoveSpeed;
    
        public NavMeshAgent AIAgent { get; private set; }
        
        [SerializeField] private Transform[] _patrolPoints;
        private int currentPatrolIndex;
        
    
        public EntityStateMachine StateMachine { get; private set; }

        protected virtual void Awake()
        {
            StateMachine = new EntityStateMachine();
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
    }
}
