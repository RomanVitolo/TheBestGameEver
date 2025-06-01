using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [CreateAssetMenu(fileName = "Item Cloth", menuName = "Core/InventoryAPI/ItemCloth")]
    public class ItemClothSO : Item
    {
        [field: SerializeField] public ItemClothTypeEnum ItemCloth { get; private set; }
        [field: SerializeField] public bool CanBeStored {get; set;}
        public enum ItemClothTypeEnum
        {
            Armor,
            Helmet,
            SpecialCloth,
            MoreTypes,
            WorkerCloth,
        }
    }
}