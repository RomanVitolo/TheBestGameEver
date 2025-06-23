using System;
using System.Collections;
using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.AI.Entities.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Scripts.Runtime.AI.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        [field: SerializeField] public EntityStatsSO EntityStats { get; set; }
        [field: SerializeField] public AttackData AttackData { get; set; }
        [field: SerializeField] public float IdleTime { get; set; }
        [field: SerializeField] public float TurnSpeed { get; set; }
        [field: SerializeField] public float ChaseSpeed { get; set; }
        [field: SerializeField] public float AggressionRange { get; set; }
        [field: SerializeField] public Transform Target { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; } 
        [field: SerializeField] public NavMeshAgent AIAgent { get; private set; }
        [field: SerializeField] public bool InCombatMode { get; private set; }
        
        [SerializeField] protected Transform[] _patrolPoints;
        [SerializeField] protected int _healthPoints = 20;
        protected EntityStateMachine StateMachine { get; private set; }

        private Vector3[] _patrolPointPosition;
        private int currentPatrolIndex;
        private bool manualMovement;
        private bool manualRotation;
        
        protected virtual void Awake()
        {
            StateMachine = new EntityStateMachine();
            if (!AIAgent)
                AIAgent = GetComponent<NavMeshAgent>();
            if (!Target)
                Target = FindAnyObjectByType<Agent>().gameObject.transform;
            InitializePatrolPoints();
        }
    
        protected virtual void Start(){}

        protected virtual void Update() { }

        protected bool ShouldEnterCombatMode()
        {
            bool inAggressionRange =  Vector3.Distance(transform.position, Target.position) < AggressionRange;

            if (!inAggressionRange || InCombatMode) return false;
            EnterCombatMode();
            return true;
        }
        
        public virtual void EnterCombatMode()
        {
            InCombatMode = true;
        }

        public void AnimationTrigger() => StateMachine.CurrentState.AnimationTrigger();

        public bool TargetInAttackRange() => Vector3.Distance(transform.position, Target.position) < AttackData.AttackRange;
       
        public Vector3 GetPatrolDestination()
        {
            Vector3 destination = _patrolPointPosition[currentPatrolIndex];
            currentPatrolIndex++;
            
            if(currentPatrolIndex >= _patrolPoints.Length)
                currentPatrolIndex = 0;
            
            return destination;
        }

        private void InitializePatrolPoints()
        {
            _patrolPointPosition = new Vector3[_patrolPoints.Length];

            for (int i = 0; i < _patrolPoints.Length; i++)
            {
                _patrolPointPosition[i] = _patrolPoints[i].position;
                _patrolPoints[i].gameObject.SetActive(false);
            }
        }

        public void FaceTarget(Vector3 target)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
            
            Vector3 currentEulerAngles = transform.eulerAngles;
            
            float yRotation = Mathf.LerpAngle(currentEulerAngles.y, targetRotation.eulerAngles.y,
                TurnSpeed * Time.deltaTime);
            
            transform.rotation = Quaternion.Euler(currentEulerAngles.x, yRotation, currentEulerAngles.z);
        }
        
        public void ActivateManualMovement(bool canManualMovement) => this.manualMovement = canManualMovement;
        public bool ManualMovementActive() => manualMovement;
        public bool ManualRotationActive() => manualRotation;
        
        public void ActivateManualRotation(bool canManualRotation) => this.manualRotation = canManualRotation;

        public virtual void GetHit()
        {
            EnterCombatMode();
            _healthPoints--;
        }

        public virtual void AbilityTrigger()
        {
            StateMachine.CurrentState.AbilityTrigger();
        }

        public virtual void HitImpact(Vector3 force, Vector3 hitPoint, Rigidbody rb)
        {
            StartCoroutine(HitImpactCoroutine(force, hitPoint, rb));
        }

        private IEnumerator HitImpactCoroutine(Vector3 force, Vector3 hitPoint, Rigidbody rb)
        {
            yield return new WaitForSeconds(0.2f);
            rb.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, AggressionRange);
        }
    }
    [Serializable]
    public struct AttackData
    {
        public AttackType_Melee AttackType;
        public string AttackName;
        public float AttackRange;
        public float AttackMoveSpeed;
        public float MoveSpeed;
        public float AttackIndex;
        [Range(1,2)] public float AnimationSpeed;
    }
}
