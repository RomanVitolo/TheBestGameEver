using Core.Scripts.Runtime.Agents;
using UnityEngine;

namespace Core.Scripts.Runtime.Networking
{
    /// <summary>
    /// Places each agent at a spawn point chosen from its owner id. The mapping is deterministic, so the
    /// server and the owning client independently arrive at the same pose without an extra RPC.
    /// </summary>
    public class AgentSpawnPoints : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoints;

        private static AgentSpawnPoints _current;

        private void Awake() => _current = this;

        private void OnDestroy()
        {
            if (_current == this) _current = null;
        }

        public static void PlaceAgent(Agent agent, ulong ownerClientId)
        {
            if (_current == null || _current._spawnPoints.Length == 0) return;

            Transform spawnPoint = _current._spawnPoints[ownerClientId % (ulong)_current._spawnPoints.Length];

            // CharacterController caches its own position and will fight a direct transform write.
            CharacterController characterController = agent.CharacterController;
            bool wasEnabled = characterController.enabled;

            characterController.enabled = false;
            agent.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
            characterController.enabled = wasEnabled;
        }
    }
}
