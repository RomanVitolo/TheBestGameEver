using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    internal class AmmoPickUp : BasePickUpComponents
    {
        [SerializeField] private WeaponEnums.WeaponAmmoType _ammoType;
        [SerializeField] private int _ammoValue;

        protected void Awake() => LoadItemEffect(itemMeshRenderer, transform);

        protected override void LoadItemEffect(MeshRenderer meshRenderer, Transform itemTransform)
        {
            itemMeshRenderer = GetComponent<MeshRenderer>();
            itemMeshRenderer.material = _componentMaterial;
            
            //TODO some Visual Effect or Animation
        }
        private void OnTriggerEnter(Collider other) => PickUpBehaviour(other);
        protected override void PickUpBehaviour(Collider other)
        {
            base.PickUpBehaviour(other);
            var ammoType = other.GetComponentInChildren<IItemPickUP<WeaponEnums.WeaponAmmoType>>();

            OnDestroyObject(onPickUp, ammoType);
        }

        private void OnDestroyObject(INotifyEvent destroyable, IItemPickUP<WeaponEnums.WeaponAmmoType> itemPickUp)
        {
            destroyable.NotifyItemAction += DestroyComponent;
            itemPickUp.PickUpObject(_ammoType, _ammoValue);                   
            destroyable.NotifyItemAction -= DestroyComponent;
        }
    }
}