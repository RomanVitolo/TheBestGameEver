using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Scripts.Runtime.Agent
{
    public class AgentAim : MonoBehaviour
    {
        [Header("Aim Information")] 
        [SerializeField] private Transform _aim;
        
        [Space]
        
        [Header("Camera Information")]
        [SerializeField] private Transform _cameraTarget;
        [Range(.5f, 1f)]
        [SerializeField] private float _minCameraDistance;
        [Range(1f, 3f)]
        [SerializeField] private float _maxCameraDistance;
        [Range(3f, 5f)]
        [SerializeField] private float _cameraSensitivity;

        private RaycastHit _lastKnownMouseHit;
        
        public void UpdateAgentCameraPosition(Vector3 mousePosition, Vector2 moveInput)
        {
            _cameraTarget.position = Vector3.Lerp(_cameraTarget.position, DesiredCameraPosition(mousePosition, moveInput),
                _cameraSensitivity * Time.deltaTime);    
        }

        public void UpdateAgentAimPosition(Vector3 mousePosition)
        {
            _aim.position = mousePosition;
            _aim.position = new Vector3(_aim.position.x, transform.position.y + 1, _aim.position.z);
        }

        private Vector3 DesiredCameraPosition(Vector3 aimPosition, Vector2 moveInput)
        {
            float actualMaxCameraDistance = moveInput.y < -.5f ? _minCameraDistance : _maxCameraDistance;     
            
            Vector3 desiredCameraPosition = aimPosition;
            Vector3 aimDirection = (desiredCameraPosition - transform.position).normalized;

            float distanceToDesiredPosition = Vector3.Distance(transform.position, desiredCameraPosition);
            float clampedDistance = Mathf.Clamp(distanceToDesiredPosition, _minCameraDistance, actualMaxCameraDistance);
            
            desiredCameraPosition = transform.position + aimDirection * clampedDistance;  
            desiredCameraPosition.y = transform.position.y + 1;
            
            return desiredCameraPosition;
        }

        public RaycastHit GetMouseHitInfo(Camera agentCamera, Vector2 InputAim, LayerMask aimLayerMask)
        {
            Ray ray = agentCamera.ScreenPointToRay(InputAim);

            if (!Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask)) 
                return _lastKnownMouseHit;
            _lastKnownMouseHit = hitInfo;
            return hitInfo;
        }
    }
}