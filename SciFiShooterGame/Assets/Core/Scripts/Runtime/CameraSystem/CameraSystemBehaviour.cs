using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Utilities;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Scripts.Runtime.CameraSystem
{
    /// <summary>
    /// One camera rig per running client. The rig is a scene object, so it cannot be wired to a player in the
    /// Editor any more: the owning client binds it to its own agent at spawn via <see cref="FollowAgent"/>.
    /// </summary>
    public class CameraSystemBehaviour : GenericSingleton<CameraSystemBehaviour>
    {
        private CinemachineCamera _virtualCamera;
        private CinemachinePositionComposer _composer;

        [SerializeField] private bool _canChangeCameraDistance;
        [SerializeField] private float _distanceChangeRate;
        private float _targetCameraDistance;

        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineCamera>();
            _composer = _virtualCamera.GetComponent<CinemachinePositionComposer>();
        }

        private void LateUpdate()
        {
          CheckCameraDistance();
        }

        /// <summary>Tracks the local player's camera target and aims at its aim reticle.</summary>
        public void FollowAgent(Agent agent)
        {
            _virtualCamera.Target.TrackingTarget = agent.AgentAim.CameraTarget;
            _virtualCamera.Target.LookAtTarget = agent.AgentAim.CameraTarget;
        }

        private void CheckCameraDistance()
        {
            if (!_canChangeCameraDistance) return;

            float currentDistance = _composer.CameraDistance;

            if (Mathf.Abs(_targetCameraDistance - currentDistance) < .01f)
                return;

            _composer.CameraDistance =
                    Mathf.Lerp(currentDistance, _targetCameraDistance, _distanceChangeRate * Time.deltaTime);
        }

        public void ChangeCameraDistance(float distance) => _targetCameraDistance = distance;
    }
}
