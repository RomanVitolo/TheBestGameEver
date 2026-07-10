
namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class DeadState_Melee : EntityState
    {
        private Entity_Melee entityMelee;
        private Entity_Ragdoll ragdoll;

        private bool interactionDisabled;
        
        public DeadState_Melee(Entity entity, EntityStateMachine entityStateMachine, string animBoolName) : base(entity, entityStateMachine, animBoolName)
        {
            entityMelee = entity as Entity_Melee;
            ragdoll = entity.GetComponent<Entity_Ragdoll>();
        }

        public override void Enter()
        {
            base.Enter();

            interactionDisabled = false;

            // The Animator and the ragdoll are switched over by Entity.DieRpc, which runs on every peer.
            // Doing it here too would only affect the server's copy.
            entityMelee.MeleeAgent.isStopped = true;

            stateTimer = 1.5f;
        }

        public override void Update()
        {
            base.Update();

            /*if (stateTimer < 0 && interactionDisabled == false)
            {
                interactionDisabled = true;
                ragdoll.RagdollActive(false);
                ragdoll.CollidersActive(false);
            }*/
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}