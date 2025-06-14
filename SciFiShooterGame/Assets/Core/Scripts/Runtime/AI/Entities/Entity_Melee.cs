﻿using System.Collections.Generic;
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

    public enum EntityMelee_Type
    {
        Regular,
        Shield
    }
    public class Entity_Melee : Entity
    {
        private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");
        public IdleState_Melee IdleState { get; private set; }
        public MoveState_Melee MoveState { get; private set; }
        public RecoveryState_Melee RecoveryState { get; private set; }
        public ChaseState_Melee ChaseState { get; private set; }
        public AttackState_Melee AttackState { get; private set; }
        public DeadState_Melee DeadStateMelee { get; private set; }

        public EntityMelee_Type MeleeType;
        public List<AttackData> AttackList;
        public Transform ShieldTransform;

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
            DeadStateMelee = new DeadState_Melee(this, StateMachine, "Idle");
        }

        protected override void Start()
        {
            base.Start();
            StateMachine.Initialize(IdleState);
            InitializeSpeciality();
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

        public override void GetHit()
        {
            base.GetHit();
            
            if(_healthPoints <= 0)
                StateMachine.ChangeState(DeadStateMelee);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AttackData.AttackRange);
        }

        private void InitializeSpeciality()
        {
            if (MeleeType == EntityMelee_Type.Shield)
            {
                ShieldTransform.gameObject.SetActive(true);
                Animator.SetFloat(ChaseIndex, 1);
            }
            
        }
    }
}