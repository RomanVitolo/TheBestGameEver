using System.Linq;
using Core.Scripts.Runtime.Items;
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public class AgentWeaponPickUp : MonoBehaviour, IItemPickUP<WeaponEnums.WeaponType>, INotifyEvent
    {
        public NotifyAction NotifyItemAction { get; set; }
        [SerializeField] private string _id;
        [SerializeField] private AgentWeaponMotor _weaponMotor;
        public string ItemId => _id;
        
        private const int c_maxWeaponsSlotsAllowed = 3;
        private void Awake()
        {
            _weaponMotor ??= GetComponent<AgentWeaponMotor>();
        }
        public void PickUpObject(WeaponEnums.WeaponType weaponType, int? value = null)
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
                    NotifyItemAction?.Invoke();
                    _weaponMotor.AgentWeaponsSlot.Add(newWeapon);
                    return;
                }
            }
        }
    }
}