using System.Linq;
using Core.Scripts.Runtime.Items;
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    internal class AgentAmmoPickUp : MonoBehaviour, IItemPickUP<WeaponEnums.WeaponAmmoType>, INotifyEvent
    {
        public NotifyAction NotifyItemAction { get; set; }
        [SerializeField] private string id;
        public string ItemId => id;
        [SerializeField] private AgentWeaponMotor _weaponMotor;
        
        private void Awake() => _weaponMotor ??= GetComponent<AgentWeaponMotor>();
        
        public void PickUpObject(WeaponEnums.WeaponAmmoType ammoType, int? ammoValue = null)
        {
            if (_weaponMotor.AgentWeaponsSlot.Count < 0) return;
            
            var weaponsToUpdate = _weaponMotor.TotalWeaponsHolder
                .Where(weapon => ammoType == WeaponEnums.WeaponAmmoType.AllBullets || 
                                 weapon.WeaponDataConfiguration.AmmoType == ammoType);
          
            foreach (var weapon in weaponsToUpdate)
            {
                weapon.WeaponDataConfiguration.TotalReserveAmmo += ammoValue.Value;
                NotifyItemAction?.Invoke();
            }
        }
    } 
}