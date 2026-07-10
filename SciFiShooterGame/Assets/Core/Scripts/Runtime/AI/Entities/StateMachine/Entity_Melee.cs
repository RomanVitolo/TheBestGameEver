using System.Collections.Generic;
using Core.Scripts.Runtime.Combat;
using Core.Scripts.Runtime.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public enum AttackType_Melee
    {
        CloseAttack,
        ChargeAttack
    }

    public enum EntityMelee_Type
    {
        Regular,
        Shield,
        Dodge,
        WeaponThrow
    }

    [CreateAssetMenu(fileName = "New Entity Melee Stats", menuName = "Core/Entity/MeleeEntity", order = 0)]
    public class EntityMeleeDataSO : ScriptableObject
    {
        [field: SerializeField] public EntityMelee_Type EntityMelee_Type { get; private set; }
        [field: SerializeField] public EntityMelee_Type AttackType_Melee { get; private set; }
        
        public static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");
        public static readonly int DodgeRoll = Animator.StringToHash("Dodge");
        
        [field: SerializeField] public EntityMelee_Type MeleeType { get; private set; }
        [field: SerializeField] public float WeaponThrowSpeed { get; private set; }
        [field: SerializeField] public float WeaponThrowAimTimer { get; private set; }
        [field: SerializeField] public float WeaponThrowCooldown { get; private set; }
        [field: SerializeField] public List<AttackData> AttackList { get; private set; }
        [field: SerializeField] public float DodgeCooldown { get; private set; }
    }
    
    public class Entity_Melee : Entity
    {
        [field: SerializeField] public EntityMeleeDataSO EntityMeleeData { get; private set; }
        private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");
        private static readonly int DodgeRoll = Animator.StringToHash("Dodge");
        public IdleState_Melee IdleState { get; private set; }
        public MoveState_Melee MoveState { get; private set; }
        public RecoveryState_Melee RecoveryState { get; private set; }
        public ChaseState_Melee ChaseState { get; private set; }
        public AttackState_Melee AttackState { get; private set; }
        public DeadState_Melee DeadStateMelee { get; private set; }
        public Entity_AbilityStateMelee  EntityAbilityStateMelee{ get; private set; }
        
        public Transform WeaponThrowStartPoint;
        public float WeaponThrowSpeed;
        public float WeaponThrowAimTimer;
        public float WeaponThrowCooldown;
        
        public EntityMelee_Type MeleeType;
        public List<AttackData> AttackList;
        public Transform ShieldTransform;
        public float DodgeCooldown;

        private float _lastTimeDodge = -10f;
        private float _lastTimeWeaponThrow;

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
            EntityAbilityStateMelee = new Entity_AbilityStateMelee(this, StateMachine, "AxeThrow");
        }

        protected override void Start()
        {
            base.Start();
            InitializeSpeciality();
        }

        protected override void OnServerSpawn()
        {
            base.OnServerSpawn();
            StateMachine.Initialize(IdleState);
        }

        protected override void Update()
        {
            base.Update();

            if (!CanSimulate) return;

            StateMachine.CurrentState.Update();

            if (ShouldEnterCombatMode())
                EnterCombatMode();
        }

        public override void EnterCombatMode()
        {
            if (InCombatMode) return;

            base.EnterCombatMode();
            StateMachine.ChangeState(RecoveryState);
        }

        public override void AbilityTrigger()
        {
            base.AbilityTrigger();
            ChaseSpeed = ChaseSpeed * .6f;
            //_pulledWeapon.gameObject.SetActive(false);
        }

        public void PullWeapon()
        {
            _hiddenWeapon.gameObject.SetActive(false);
            _pulledWeapon.gameObject.SetActive(true);
        }

        protected override void OnServerDeath() => StateMachine.ChangeState(DeadStateMelee);

        /// <summary>Called on the attack animation's hit frame, server-side only.</summary>
        public void TryDealMeleeDamage()
        {
            if (!IsServer || !IsAlive || !TargetInAttackRange()) return;

            IDamageable damageable = TargetAgent.GetComponent<IDamageable>();

            if (damageable == null || !damageable.IsAlive) return;

            Vector3 hitPoint = TargetAgent.transform.position + Vector3.up;
            damageable.TakeDamage(_attackDamage, transform.forward * AttackData.AttackMoveSpeed, hitPoint);
        }

        /// <summary>Server spawns the damaging thrown weapon; clients spawn a cosmetic copy of it.</summary>
        public void ThrowWeapon()
        {
            if (!IsServer || TargetAgent == null) return;

            NetworkObjectReference targetReference = TargetAgent.NetworkObject;

            SpawnThrownWeapon(targetReference, isAuthoritative: true);
            SpawnCosmeticThrownWeaponRpc(targetReference);
        }

        [Rpc(SendTo.NotServer)]
        private void SpawnCosmeticThrownWeaponRpc(NetworkObjectReference targetReference) =>
            SpawnThrownWeapon(targetReference, isAuthoritative: false);

        private void SpawnThrownWeapon(NetworkObjectReference targetReference, bool isAuthoritative)
        {
            if (!targetReference.TryGet(out NetworkObject targetObject)) return;

            Entity_WeaponThrow thrownWeapon = GlobalPoolContainer.Instance.WeaponThrow.GetObject();
            thrownWeapon.transform.position = WeaponThrowStartPoint.position;

            thrownWeapon.WeaponThrowSetup(WeaponThrowSpeed, targetObject.transform, WeaponThrowAimTimer,
                isAuthoritative, _attackDamage);
        }

        public void ActivateDodgeRoll()
        {
            // Called from the server's hit resolution; a client deciding this alone would desync the enemy.
            if (!CanSimulate) return;
            if (MeleeType != EntityMelee_Type.Dodge) return;
            if (StateMachine.CurrentState != ChaseState) return;
            if (Vector3.Distance(transform.position, Target.position) < 2f) return;

            float dodgeAnimationDuration = GetAnimationClipDuration(DodgeRoll.ToString());
            
            if (!(Time.time > DodgeCooldown + dodgeAnimationDuration + _lastTimeDodge)) return;
            _lastTimeDodge = Time.time;
            Animator.SetTrigger(DodgeRoll);
        }

        public bool CanThrowWeapon()
        {
            if (MeleeType != EntityMelee_Type.WeaponThrow) return false;
            
            if (Time.time > _lastTimeWeaponThrow + WeaponThrowCooldown)
            {
                _lastTimeWeaponThrow = Time.time;
                return true;
            }
            return false;
        }

        private float GetAnimationClipDuration(string clipName)
        {
            AnimationClip[] clips = Animator.runtimeAnimatorController.animationClips;

            foreach (AnimationClip clip in clips)
            {
                if(clip.name == clipName)
                    return clip.length;
            }

            Debug.Log($"{clipName} No such animation clip found");
            return 0f;
        }

        private void InitializeSpeciality()
        {
            if (MeleeType == EntityMelee_Type.Shield)
            {
                ShieldTransform.gameObject.SetActive(true);
                Animator.SetFloat(ChaseIndex, 1);
            }
            
        }
        
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, AttackData.AttackRange);
        }
    }
}