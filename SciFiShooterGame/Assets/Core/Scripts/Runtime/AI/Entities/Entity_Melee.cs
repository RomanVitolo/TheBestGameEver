using System.Collections.Generic;
using Core.Scripts.Runtime.AI.Entities.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace Core.Scripts.Runtime.AI.Entities
{
    public enum AttackType_Melee
    {
        CloseAttack,
        ChargeAttack
    }
    
    public class Entity_Melee : Entity
    {
        public IdleState_Melee IdleState { get; private set; }
        public MoveState_Melee MoveState { get; private set; }
        public RecoveryState_Melee RecoveryState { get; private set; }
        public ChaseState_Melee ChaseState { get; private set; }
        public AttackState_Melee AttackState { get; private set; }

        public List<AttackData> AttackList;

        [SerializeField] private Transform _hiddenWeapon;
        [SerializeField] private Transform _pulledWeapon;

        public NavMeshAgent MeleeAgent => AIAgent;

        protected override void Awake()
        {
            base.Awake();
            
            IdleState = new IdleState_Melee(this, StateMachine, "Idle");
            MoveState = new MoveState_Melee(this, StateMachine, "Move");
            RecoveryState = new RecoveryState_Melee(this, StateMachine, "Recovery");
            ChaseState = new ChaseState_Melee(this, StateMachine, "Chase");
            AttackState = new AttackState_Melee(this, StateMachine, "Attack");
        }

        protected override void Start()
        {
            base.Start();
            
            StateMachine.Initialize(IdleState);
        }

        protected override void Update()
        {
            base.Update();
            
            StateMachine.CurrentState.Update();
        }

        public void PullWeapon()
        {
            _hiddenWeapon.gameObject.SetActive(false);
            _pulledWeapon.gameObject.SetActive(true);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AttackData.AttackRange);
        }
    }
}