using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    internal class WeaponPickUp : BasePickUpComponents
    {
        [SerializeField] private WeaponEnums.WeaponType _weaponId;
        private MeshRenderer _weaponRender;

        protected void Awake() => LoadItemEffect(_weaponRender, this.transform);

        private void OnTriggerEnter(Collider other) => PickUpBehaviour(other);

        private void PickUpBehaviour(Collider other)
        {
            var destroyable = other.GetComponentInChildren<NotifyEvent>();
            var itemPickUp = other.GetComponentInChildren<IItemPickUP<WeaponEnums.WeaponType>>();

            if (destroyable is null && itemPickUp is null) return;
            KillItemEffect(this.transform);
            OnDestroyObject(destroyable, itemPickUp);
        }

        private void OnDestroyObject(NotifyEvent destroyable, IItemPickUP<WeaponEnums.WeaponType> itemPickUp)
        {
            destroyable.NotifyAction += DestroyComponent;
            itemPickUp.PickUpObject(_weaponId);                   
            destroyable.NotifyAction -= DestroyComponent;
        }
    }       
}