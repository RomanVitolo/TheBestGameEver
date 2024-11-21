using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    public class WeaponPickUp : MonoBehaviour
    {
        [SerializeField] private WeaponEnums.WeaponType _weaponId; 

        private void DestroyComponent()
        {
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {  
            if (other == null) return;        
           
            var destroyable = other.GetComponentInChildren<UtilityEvent>();
            var itemPickUp = other.GetComponentInChildren<IItemPickUP<WeaponEnums.WeaponType>>();

            if (destroyable == null || itemPickUp == null) return;  
            
            destroyable.NotifyAction += DestroyComponent;
            itemPickUp.PickUpObject(_weaponId);                       
            destroyable.NotifyAction -= DestroyComponent;
        }         
    }       
}