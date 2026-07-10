using System;
using System.Collections;
using Core.Scripts.Runtime.Combat;
using Core.Scripts.Runtime.Networking;
using Unity.Netcode;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    /// <summary>
    /// Server-authoritative player health. Movement stays owner-authoritative, but only the server may
    /// subtract health, so a client cannot decide it survived a hit.
    /// </summary>
    [RequireComponent(typeof(Agent))]
    public class AgentHealth : NetworkBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private float _respawnDelay = 3f;

        private readonly NetworkVariable<int> _health = new();

        private Agent _agent;
        private AgentMotor _agentMotor;
        private AgentWeaponMotor _agentWeaponMotor;

        public int Health => _health.Value;
        public int MaxHealth => _maxHealth;
        public bool IsAlive => _health.Value > 0;

        /// <summary>Raised on every peer when this agent dies or respawns.</summary>
        public event Action<bool> AliveChanged;

        private void Awake()
        {
            _agent = GetComponent<Agent>();
            _agentMotor = GetComponent<AgentMotor>();
            _agentWeaponMotor = GetComponentInChildren<AgentWeaponMotor>(true);
        }

        public override void OnNetworkSpawn()
        {
            _health.OnValueChanged += OnHealthChanged;

            if (IsServer)
                _health.Value = _maxHealth;
        }

        public override void OnNetworkDespawn() => _health.OnValueChanged -= OnHealthChanged;

        public void TakeDamage(int amount, Vector3 force, Vector3 hitPoint)
        {
            if (!IsServer || !IsAlive || amount <= 0) return;

            _health.Value = Mathf.Max(0, _health.Value - amount);

            if (_health.Value == 0)
                StartCoroutine(RespawnAfterDelay());
        }

        private void OnHealthChanged(int previous, int current)
        {
            if (previous > 0 && current == 0) SetAlive(false);
            else if (previous == 0 && current > 0) SetAlive(true);
        }

        private void SetAlive(bool alive)
        {
            // Handlers only ever run on the owner anyway, but disabling them everywhere keeps a dead remote
            // agent from animating as though it were still walking.
            if (_agentMotor != null) _agentMotor.enabled = alive;
            if (_agentWeaponMotor != null) _agentWeaponMotor.enabled = alive;

            _agent.AgentAim.SetAimVisualsEnabled(alive && IsOwner);

            AliveChanged?.Invoke(alive);
        }

        private IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(_respawnDelay);

            if (!IsSpawned) yield break;

            _health.Value = _maxHealth;
            RespawnRpc();
        }

        // Only the owner may move an owner-authoritative NetworkTransform, so the server asks it to teleport.
        [Rpc(SendTo.Owner)]
        private void RespawnRpc() => AgentSpawnPoints.PlaceAgent(_agent, OwnerClientId);
    }
}
