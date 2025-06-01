using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [CreateAssetMenu(fileName = "Item Magazine", menuName = "Core/InventoryAPI/ItemMagazine")]
    public class ItemMagazineSO : Item
    {
        [field: SerializeField] public ItemMagazineTypeEnum MagazineType { get; private set;}
        [field: SerializeField] public int AmmoCapacity { get; set;}
        public enum ItemMagazineTypeEnum
        {
            PistolMagazine,
            ShotgunMagazine,
            RifleMagazine,
            SniperMagazine,
            SpecialMagazine,
        }
    }
}