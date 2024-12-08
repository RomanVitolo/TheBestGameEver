using System;
using System.Linq;
using Core.Scripts.Runtime.Items;
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public class AgentWeaponPickUp : MonoBehaviour, IItemPickUP<WeaponEnums.WeaponType>, NotifyEvent
    {
        public event Action NotifyAction;
        
        [SerializeField] private AgentWeaponMotor _weaponMotor;
        private const int c_maxWeaponsSlotsAllowed = 3;
        private void Awake()
        {
            if (_weaponMotor == null)
                _weaponMotor = GetComponent<AgentWeaponMotor>();
        }

        public void PickUpObject(WeaponEnums.WeaponType weaponType)
        {
            if (_weaponMotor.AgentWeaponsSlot.Count >= c_maxWeaponsSlotsAllowed)
                return;
            foreach (var weapon in _weaponMotor.AgentWeaponsSlot)
            {
                if (weapon.WeaponDataConfiguration.WeaponType == weaponType || _weaponMotor.AgentWeaponsSlot.Exists(w => 
                        w.WeaponDataConfiguration.WeaponType == weaponType))
                    return;
                foreach (var newWeapon in _weaponMotor.TotalWeaponsHolder.
                             Where(newWeapon => newWeapon.WeaponDataConfiguration.WeaponType == weaponType))
                {
                    NotifyAction?.Invoke();
                    _weaponMotor.AgentWeaponsSlot.Add(newWeapon);
                    return;
                }
            }
        }
    }
}