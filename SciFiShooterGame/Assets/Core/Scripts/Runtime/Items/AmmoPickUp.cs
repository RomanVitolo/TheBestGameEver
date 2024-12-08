using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    internal class AmmoPickUp : BasePickUpComponents
    {
        [SerializeField] private WeaponEnums.WeaponAmmoType _ammoType;
        private MeshRenderer _weaponRender;

        protected void Awake()
        {
            LoadItemEffect(_weaponRender, transform);
        }

        private void OnTriggerEnter(Collider other)
        {
            PickUpBehaviour(other);
        }

        private void PickUpBehaviour(Collider other)
        {
            var destroyable = other.GetComponentInChildren<NotifyEvent>();
            var itemPickUp = other.GetComponentInChildren<IItemPickUP<WeaponEnums.WeaponAmmoType>>();

            if (destroyable is null && itemPickUp is null) return;
            KillItemEffect(transform);
            OnDestroyObject(destroyable, itemPickUp);
        }

        private void OnDestroyObject(NotifyEvent destroyable, IItemPickUP<WeaponEnums.WeaponAmmoType> itemPickUp)
        {
            destroyable.NotifyAction += DestroyComponent;
            itemPickUp.PickUpObject(_ammoType);                   
            destroyable.NotifyAction -= DestroyComponent;
        }
    }
}