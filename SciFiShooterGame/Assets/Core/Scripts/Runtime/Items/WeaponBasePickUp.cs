using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    internal class WeaponBasePickUp : BasePickUpComponents
    {
        [SerializeField] private WeaponEnums.WeaponType _weaponId; 

        private void OnTriggerEnter(Collider other)
        {
            PickUpBehaviour(other);
        }

        private void PickUpBehaviour(Collider other)
        {
            
            var destroyable = other.GetComponentInChildren<IUtilityEvent>();
            var itemPickUp = other.GetComponentInChildren<IItemPickUP<WeaponEnums.WeaponType>>();

            if (destroyable is not null || itemPickUp is not null)
                OnDestroyObject(destroyable, itemPickUp);
        }

        private void OnDestroyObject(IUtilityEvent destroyable, IItemPickUP<WeaponEnums.WeaponType> itemPickUp)
        {
            destroyable.NotifyAction += DestroyComponent;
            itemPickUp.PickUpObject(_weaponId);                       
            destroyable.NotifyAction -= DestroyComponent;
        }
    }       
}