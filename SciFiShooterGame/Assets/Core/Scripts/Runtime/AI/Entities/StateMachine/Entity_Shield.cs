using Unity.Netcode;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    /// <summary>
    /// A NetworkBehaviour on a child of the entity's NetworkObject, so durability replicates. The shield is
    /// deactivated rather than destroyed: destroying a NetworkBehaviour at runtime shifts the indices NGO
    /// uses to route messages to its siblings.
    /// </summary>
    public class Entity_Shield : NetworkBehaviour
    {
        private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");

        [field: SerializeField] public int ShieldDurability { get; set; }

        private readonly NetworkVariable<int> _durability = new();

        private Entity_Melee _entityMelee;

        private void Awake()
        {
            _entityMelee = GetComponentInParent<Entity_Melee>();
        }

        public override void OnNetworkSpawn()
        {
            _durability.OnValueChanged += OnDurabilityChanged;

            if (IsServer)
                _durability.Value = ShieldDurability;
        }

        public override void OnNetworkDespawn() => _durability.OnValueChanged -= OnDurabilityChanged;

        public void ReduceDurability()
        {
            if (!IsServer || _durability.Value <= 0) return;

            _durability.Value--;
        }

        private void OnDurabilityChanged(int previous, int current)
        {
            ShieldDurability = current;

            if (current > 0) return;

            if (_entityMelee != null && _entityMelee.Animator != null)
                _entityMelee.Animator.SetFloat(ChaseIndex, 0);

            gameObject.SetActive(false);
        }
    }
}
