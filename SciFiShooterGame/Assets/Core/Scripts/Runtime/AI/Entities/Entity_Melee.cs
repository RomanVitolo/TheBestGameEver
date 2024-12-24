using Core.Scripts.Runtime.AI.StateMachine;

namespace Core.Scripts.Runtime.AI.Entities
{
    public class Entity_Melee : Entity
    {
        public IdleState_Melee IdleState { get; private set; }
        public MoveState_Melee MoveState { get; set; }

        protected override void Awake()
        {
            base.Awake();
            
            IdleState = new IdleState_Melee(this, StateMachine, "Idle");
            MoveState = new MoveState_Melee(this, StateMachine, "Move");
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
    }
}