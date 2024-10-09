using Core.Scripts.Runtime.Agents.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public class AgentAim : MonoBehaviour, IAgentAim
    {
        [SerializeField] private LineRenderer _aimLaser; 
        
        [Header("Aim Information")] 
        [field: SerializeField] public Transform Aim { get; set; }

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
                Aim.position = target.position;
                return;
            } 
            
            Aim.position = mousePosition;
            
            if(!_isAimingPrecisely)
                Aim.position = new Vector3(Aim.position.x, transform.position.y + 1, Aim.position.z);
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

        public bool CanAimPrecisely() => _isAimingPrecisely;  


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
            Transform gunPosition = gunPoint;
            Vector3 laserDirection = bulletDirection;
            
            float laserTipLenght = .5f;
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
    }
}
