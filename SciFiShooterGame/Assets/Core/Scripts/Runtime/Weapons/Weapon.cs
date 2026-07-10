using Core.Scripts.Runtime.Utilities;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform _weaponGunPoint;

        [Header("Weapon Configuration")]
        [field: SerializeField] public WeaponDataSO WeaponDataConfiguration { get; private set; }

        public WeaponRuntime Runtime { get; private set; }

        private void Awake() => EnsureRuntime();

        // Weapons that start deactivated never run Awake, and AgentWeaponPickUp can pull them out of
        // TotalWeaponsHolder later, so guarantee the runtime exists the moment the object is switched on.
        private void OnEnable() => EnsureRuntime();

        private void EnsureRuntime()
        {
            if (Runtime != null) return;

            if (_weaponGunPoint == null)
                _weaponGunPoint = GetComponentInChildren<GunPointTransform>().transform;

            Runtime = new WeaponRuntime(WeaponDataConfiguration, _weaponGunPoint);
        }
    }
}
