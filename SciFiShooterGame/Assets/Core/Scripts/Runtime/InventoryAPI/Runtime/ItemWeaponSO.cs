using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [CreateAssetMenu(fileName = "Item Weapon", menuName = "Core/InventoryAPI/ItemWeapon")]
    public class ItemWeaponSO : Item
    {
        [field: SerializeField] public ItemWeaponTypeEnum WeaponType { get; private set; }
        [field: SerializeField] public bool CanBeCombine { get; private set; }
        [field: SerializeField] public int EffectiveRange { get; set; }
        public enum ItemWeaponTypeEnum
        {
            MeleeWeapon,
            ThrowableWeapon,
            Pistol,
            Shotgun,
            Rifle,
            Sniper,
            SpecialWeapon,
        }
    }
}