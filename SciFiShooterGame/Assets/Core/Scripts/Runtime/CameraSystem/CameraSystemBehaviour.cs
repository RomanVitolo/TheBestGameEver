using Core.Scripts.Runtime.Utilities;
using Unity.Cinemachine;
using UnityEngine;

namespace Core.Scripts.Runtime.CameraSystem
{
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

