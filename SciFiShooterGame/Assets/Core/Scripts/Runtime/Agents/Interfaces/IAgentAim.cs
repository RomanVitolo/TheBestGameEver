using UnityEngine;

namespace Core.Scripts.Runtime.Agents.Interfaces
{
    public interface IAgentAim
    {
        public Transform Aim { get; set; }
        /// <summary>What the owning client's Cinemachine camera tracks.</summary>
        public Transform CameraTarget { get; }
        public void SetAimVisualsEnabled(bool isEnabled);
        public void DetachWorldMarkers();
        public void UpdateAgentCameraPosition(Vector3 mousePosition, Vector2 moveInput);
        public void UpdateAgentAimPosition(Vector3 mousePosition, RaycastHit targetTransform);
        public Transform Target(RaycastHit targetTransform);
        public bool CanAimPrecisely();
        public RaycastHit GetMouseHitInfo(Camera agentCamera, Vector2 InputAim, LayerMask aimLayerMask);
        public void UpdateAimVisuals(Transform gunPoint, Vector3 bulletDirection, bool weaponIsReady, float weaponDistance);
    }
}