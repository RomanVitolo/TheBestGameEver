using Core.Scripts.Runtime.Ammo;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public class WeaponBulletMovement : MonoBehaviour
    {
        private Agent _agent;
        
        [Header("Ammo Settings")]
        [field: SerializeField] public float BulletSpeed { get; private set; }

        private void Awake()
        {
            _agent = GetComponentInParent<Agent>();
        }

        public Vector3 BulletDirection(Vector3 weaponPosition)
        {
            Transform aim = _agent.AgentAim.Aim;

            Vector3 direction = (aim.position - weaponPosition).normalized;

            if (!_agent.AgentAim.CanAimPrecisely() &&
                _agent.AgentAim.Target(_agent.AgentAim.GetMouseHitInfo(Camera.main,
                    _agent.AgentInputReader.AimInputValue, _agent.AgentMovement.AimLayerMask)) == null)
                direction.y = 0;

            return direction;
        }
    }
}