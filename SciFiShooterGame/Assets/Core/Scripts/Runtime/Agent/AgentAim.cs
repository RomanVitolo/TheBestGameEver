using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Scripts.Runtime.Agent
{
    public class AgentAim : MonoBehaviour
    {
        [SerializeField] private LineRenderer _aimLaser; 
        
        [Header("Aim Information")] 
        [SerializeField] private Transform _aim;

        [SerializeField] private bool _isAimingPrecisely;
        [SerializeField] private bool _isLockingToTarget;
        
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

        public void UpdateAgentAimPosition(Vector3 mousePosition, RaycastHit targetTransform)
        {
            Transform target = Target(targetTransform);

            if (target != null && _isLockingToTarget)
            {
                _aim.position = target.position;
                return;
            } 
            
            _aim.position = mousePosition;
            
            if(!_isAimingPrecisely)
                _aim.position = new Vector3(_aim.position.x, transform.position.y + 1, _aim.position.z);
        }

        public Transform Target(RaycastHit targetTransform)
        {
            Transform target = null;

            if (targetTransform.transform.GetComponent<Target>() != null)
            {
                target = targetTransform.transform;
            }

            return target;
        }

        public bool CanAimPrecisely()
        {
            if (_isAimingPrecisely)
                return true;

            return false;
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

        public void UpdateAimLaser(Transform gunPoint, Vector3 bulletDirection)
        {
            float laserTipLenght = .5f;
            
            Transform gunPosition = gunPoint;
            Vector3 laserDirection = bulletDirection;
            float gunDistance = 4f;
            
            Vector3 endPoint = gunPoint.position + laserDirection * gunDistance;

            if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hit, gunDistance))
            {
                endPoint = hit.point;
                laserTipLenght = 0;
            }
                
            
            _aimLaser.SetPosition(0, gunPosition.position);
            _aimLaser.SetPosition(1, endPoint);
            _aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLenght);
        }
    }
}