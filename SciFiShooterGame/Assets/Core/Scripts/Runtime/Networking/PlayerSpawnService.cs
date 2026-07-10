using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scripts.Runtime.Networking
{
    /// <summary>
    /// Spawns player objects only once the combat scene is live.
    ///
    /// NetworkManager's own PlayerPrefab field must stay empty: it spawns a player the moment a peer
    /// connects, which happens while the menu scene is still loaded. Agent.OnNetworkSpawn would then find no
    /// AgentSpawnPoints and no CameraSystemBehaviour, and its one-shot camera binding would never happen.
    ///
    /// Lives on the NetworkManager GameObject, so it survives the scene load.
    /// </summary>
    public class PlayerSpawnService : MonoBehaviour
    {
        [SerializeField] private NetworkObject _playerPrefab;

        private readonly HashSet<ulong> _spawnedClients = new();
        private bool _combatSceneLoaded;

        private void Awake() => NetworkManager.Singleton.OnServerStarted += OnServerStarted;

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null) return;

            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

            if (NetworkManager.Singleton.SceneManager == null) return;

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= OnSynchronizeComplete;
        }

        private void OnServerStarted()
        {
            // SceneManager only exists once the server is up.
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSynchronizeComplete += OnSynchronizeComplete;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        /// <summary>Server-side, once every already-connected client has finished loading the scene.</summary>
        private void OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode,
            List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            if (sceneName != GameScenes.Combat) return;

            _combatSceneLoaded = true;

            foreach (ulong clientId in clientsCompleted)
                SpawnPlayerFor(clientId);
        }

        /// <summary>
        /// A client that joins after the match started never raises OnLoadEventCompleted: it is synchronized
        /// into the running scene instead. OnClientConnectedCallback would fire too early, before it has
        /// actually loaded the scene its player belongs in.
        /// </summary>
        private void OnSynchronizeComplete(ulong clientId)
        {
            if (!_combatSceneLoaded) return;

            SpawnPlayerFor(clientId);
        }

        private void OnClientDisconnected(ulong clientId) => _spawnedClients.Remove(clientId);

        private void SpawnPlayerFor(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer || !_spawnedClients.Add(clientId)) return;

            // Agent.OnNetworkSpawn places itself at its spawn point, so no pose is needed here.
            NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(
                _playerPrefab, clientId, destroyWithScene: false, isPlayerObject: true);
        }
    }
}
