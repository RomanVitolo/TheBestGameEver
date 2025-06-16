using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    internal class WeaponPickUp : BasePickUpComponents
    {
        [SerializeField] private WeaponEnums.WeaponType _weaponId;

        protected void Awake() => LoadItemEffect(itemMeshRenderer, this.transform);

        private void OnTriggerEnter(Collider other) => PickUpBehaviour(other);

        protected override void PickUpBehaviour(Collider other)
        {
            var itemPickUp = other.GetComponentInChildren<IItemPickUP<WeaponEnums.WeaponType>>();
            base.PickUpBehaviour(other);
            
            KillItemEffect(this.transform);
            OnDestroyObject(onPickUp, itemPickUp);
        }

        private void OnDestroyObject(INotifyEvent destroyable, IItemPickUP<WeaponEnums.WeaponType> itemPickUp)
        {
            if(destroyable == null) return;
            destroyable.NotifyItemAction += DestroyComponent;
            itemPickUp?.PickUpObject(_weaponId);                   
            destroyable.NotifyItemAction -= DestroyComponent;
        }
    }       
}