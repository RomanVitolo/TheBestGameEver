using System;
using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.AI.Entities.StateMachine;
using Core.Scripts.Runtime.Combat;
using Core.Scripts.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Scripts.Runtime.AI.Entities
{
    /// <summary>
    /// Server-authoritative AI. The state machine, the NavMeshAgent and all damage run on the server only;
    /// clients receive the result through NetworkTransform, NetworkAnimator and <see cref="NetworkHealth"/>.
    /// Simulated locally on each peer these would diverge within seconds — the states pick attacks and dodge
    /// rolls with Random, and NavMesh pathing depends on frame timing.
    /// </summary>
    public abstract class Entity : NetworkBehaviour, IDamageable
    {
        [field: SerializeField] public EntityStatsSO EntityStats { get; set; }
        [field: SerializeField] public AttackData AttackData { get; set; }
        [field: SerializeField] public float IdleTime { get; set; }
        [field: SerializeField] public float TurnSpeed { get; set; }
        [field: SerializeField] public float ChaseSpeed { get; set; }
        [field: SerializeField] public float AggressionRange { get; set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public NavMeshAgent AIAgent { get; private set; }
        [field: SerializeField] public bool InCombatMode { get; private set; }

        [SerializeField] protected Transform[] _patrolPoints;
        [SerializeField] protected int _healthPoints = 20;
        [SerializeField] protected int _attackDamage = 10;
        [SerializeField] private float _retargetInterval = 0.5f;

        public readonly NetworkVariable<int> NetworkHealth = new();
        protected EntityStateMachine StateMachine { get; private set; }

        private Entity_Ragdoll _ragdoll;
        private Vector3[] _patrolPointPosition;
        private int currentPatrolIndex;
        private bool manualMovement;
        private bool manualRotation;
        private float _lastRetargetTime;
        private bool _deathVisualsApplied;

        /// <summary>Chosen by the server, never by a client.</summary>
        public Agent TargetAgent { get; private set; }
        public Transform Target => TargetAgent != null ? TargetAgent.transform : null;

        public bool IsAlive => NetworkHealth.Value > 0;

        /// <summary>True only where this entity is allowed to think.</summary>
        protected bool CanSimulate => IsSpawned && IsServer && IsAlive && Target != null;

        protected virtual void Awake()
        {
            StateMachine = new EntityStateMachine();

            if (!AIAgent)
                AIAgent = GetComponent<NavMeshAgent>();

            _ragdoll = GetComponent<Entity_Ragdoll>();

            InitializePatrolPoints();
        }

        protected virtual void Start(){}

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                // A client-side NavMeshAgent would fight the replicated transform for control.
                if (AIAgent != null) AIAgent.enabled = false;
                return;
            }

            NetworkHealth.Value = _healthPoints;
            AcquireNearestTarget();
            OnServerSpawn();
        }

        /// <summary>Server-side setup. In-scene entities Awake long before a session exists.</summary>
        protected virtual void OnServerSpawn() {}

        protected virtual void Update()
        {
            if (!IsSpawned || !IsServer || !IsAlive) return;

            RetargetIfDue();
        }

        private void RetargetIfDue()
        {
            if (Time.time < _lastRetargetTime + _retargetInterval) return;

            _lastRetargetTime = Time.time;
            AcquireNearestTarget();
        }

        private void AcquireNearestTarget() => TargetAgent = AgentRegistry.GetNearestAlive(transform.position);

        protected bool ShouldEnterCombatMode()
        {
            if (Target == null) return false;

            bool inAggressionRange = Vector3.Distance(transform.position, Target.position) < AggressionRange;

            if (!inAggressionRange || InCombatMode) return false;
            EnterCombatMode();
            return true;
        }

        public virtual void EnterCombatMode()
        {
            InCombatMode = true;
        }

        public void AnimationTrigger()
        {
            // NetworkAnimator replays clips on every peer, so animation events fire everywhere. The state
            // machine only exists on the server.
            if (!IsServer || StateMachine.CurrentState == null) return;

            StateMachine.CurrentState.AnimationTrigger();
        }

        public bool TargetInAttackRange() =>
            Target != null && Vector3.Distance(transform.position, Target.position) < AttackData.AttackRange;

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

        public void TakeDamage(int amount, Vector3 force, Vector3 hitPoint)
        {
            if (!IsServer || !IsAlive || amount <= 0) return;

            EnterCombatMode();

            NetworkHealth.Value = Mathf.Max(0, NetworkHealth.Value - amount);

            if (NetworkHealth.Value == 0)
                DieRpc(force, hitPoint);
        }

        public virtual void AbilityTrigger()
        {
            if (!IsServer || StateMachine.CurrentState == null) return;

            StateMachine.CurrentState.AbilityTrigger();
        }

        [Rpc(SendTo.Everyone)]
        private void DieRpc(Vector3 force, Vector3 hitPoint)
        {
            ApplyDeathVisuals(force, hitPoint);

            if (IsServer)
                OnServerDeath();
        }

        /// <summary>Ragdolls locally on every peer. Bone poses drift apart; only the root is replicated.</summary>
        private void ApplyDeathVisuals(Vector3 force, Vector3 hitPoint)
        {
            if (_deathVisualsApplied) return;
            _deathVisualsApplied = true;

            if (Animator != null) Animator.enabled = false;

            if (_ragdoll == null) return;

            _ragdoll.RagdollActive(true);
            _ragdoll.ApplyImpulse(force, hitPoint);
        }

        protected virtual void OnServerDeath() {}

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
