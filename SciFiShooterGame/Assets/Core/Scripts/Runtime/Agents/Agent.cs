using Core.Scripts.Runtime.Agents.Interfaces;
using Core.Scripts.Runtime.CameraSystem;
using Core.Scripts.Runtime.Networking;
using GlobalInputs;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    [RequireComponent(typeof(CharacterController), typeof(NetworkObject), typeof(AgentInputReader))]
    [RequireComponent(typeof(AgentHealth), typeof(AgentWeaponFire))]
    public class Agent : NetworkBehaviour
    {
        [field: SerializeField, Header("Agent Movement"), Space] public AgentMovement AgentMovement { get; set; }
        [field: SerializeField, Header("Agent Animations"), Space] public AgentAnimatorSO AgentAnimator { get; private set; }

        public AgentInputReader AgentInputReader { get; private set; }
        public IAgentAim AgentAim { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public Animator Animator { get; private set; }
        /// <summary>Replicates triggers. Only the authority (the owner, on this prefab) may call SetTrigger.</summary>
        public NetworkAnimator NetworkAnimator { get; private set; }
        public AgentHealth Health { get; private set; }
        public AgentWeaponFire WeaponFire { get; private set; }

        /// <summary>The camera this agent aims through. Only ever assigned on the owning client.</summary>
        public Camera AgentCamera { get; private set; }

        private void Awake()
        {
            AgentInputReader = GetComponent<AgentInputReader>();
            CharacterController = GetComponent<CharacterController>();
            AgentAim = GetComponent<IAgentAim>();
            Animator = GetComponentInChildren<Animator>();
            // The NetworkAnimator lives on the character model child (next to the Animator), not the root.
            NetworkAnimator = GetComponentInChildren<NetworkAnimator>();
            Health = GetComponent<AgentHealth>();
            WeaponFire = GetComponent<AgentWeaponFire>();
        }

        public override void OnNetworkSpawn()
        {
            // Registered on every peer: the server picks enemy targets from this list, and it is what
            // replaced the FindAnyObjectByType<Agent>() that enemies used to call in Awake.
            AgentRegistry.Register(this);

            // The server writes the pose so late joiners get it in the initial spawn state; the owner writes
            // it too, because the NetworkTransform on this prefab is owner-authoritative.
            if (IsServer || IsOwner)
                AgentSpawnPoints.PlaceAgent(this, OwnerClientId);

            if (!IsOwner)
            {
                AgentAim.SetAimVisualsEnabled(false);
                return;
            }

            // After PlaceAgent, so the markers detach at the spawn point rather than at the prefab origin.
            AgentAim.DetachWorldMarkers();

            AgentInputReader.InitializeControls();
            AgentCamera = Camera.main;

            // Deliberately not CameraSystemBehaviour.Instance: GenericSingleton would fabricate an empty
            // GameObject when the rig is absent, and its Awake needs a CinemachineCamera to exist.
            var cameraSystem = FindFirstObjectByType<CameraSystemBehaviour>();

            if (cameraSystem != null)
                cameraSystem.FollowAgent(this);
            else
                Debug.LogWarning($"{nameof(Agent)}: no {nameof(CameraSystemBehaviour)} in the scene; " +
                                 "the local player's camera will not follow it.", this);
        }

        public override void OnNetworkDespawn()
        {
            AgentRegistry.Unregister(this);

            if (IsOwner)
                AgentInputReader.DestroyControls();
        }
    }
}
