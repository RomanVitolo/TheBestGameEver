using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [CreateAssetMenu(fileName = "Item Money", menuName = "Core/InventoryAPI/ItemMoney")]
    public class ItemMoney : Item
    {
        [field: SerializeField] public ItemMoneyTypeEnum MoneyTypeEnum { get; set; }
        [field: SerializeField] public string MoneySymbol { get; set; }
        [field: SerializeField] public int MoneyAmount { get; set; }
        
        public enum ItemMoneyTypeEnum
        {
            UniversalGalaxyMoney,
            GalaxyOneMoney,
            GalaxyTwoMoney,
        }
    }
}