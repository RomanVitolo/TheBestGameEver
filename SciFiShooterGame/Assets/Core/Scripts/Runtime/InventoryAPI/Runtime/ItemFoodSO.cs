using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [CreateAssetMenu(fileName = "Item Food", menuName = "Core/InventoryAPI/ItemFood")]
    public class ItemFoodSO : Item
    {
        [field: SerializeField] public ItemFoodTypeEnum ItemFood { get; private set; }
        [field: SerializeField] public int Energy { get; set; }
        public enum ItemFoodTypeEnum
        {
            NormalFood,
            SpecialFood,
            Drink
        }
    }
}