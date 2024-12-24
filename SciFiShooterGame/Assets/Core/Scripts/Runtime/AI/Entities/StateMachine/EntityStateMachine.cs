namespace Core.Scripts.Runtime.AI.StateMachine
{
    public class EntityStateMachine
    {
        public EntityState CurrentState { get; private set; }

        public void Initialize(EntityState initialState)
        {
            CurrentState = initialState;
            CurrentState.Enter();
        }

        public void ChangeState(EntityState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
        }
    }
}