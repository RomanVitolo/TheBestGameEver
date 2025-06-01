using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [CreateAssetMenu(fileName = "ItemAmmo", menuName = "Core/InventoryAPI/ItemAmmo")]
    public class ItemAmmoSO : Item
    {
        [field: SerializeField] public ItemAmmoTypeEnum AmmoType {get; private set;}
        [field: SerializeField] public int Damage {get; set;}
        public enum ItemAmmoTypeEnum
        {
            NormalAmmo,
            SmgAmmo,
            RifleAmmo,
            ShotgunAmmo,
            SniperAmmo,
            PistolAmmo,
            SpecialAmmo
        }
    }
}