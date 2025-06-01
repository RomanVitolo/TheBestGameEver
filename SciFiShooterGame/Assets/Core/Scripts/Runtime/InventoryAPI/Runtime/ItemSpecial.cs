using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [CreateAssetMenu(fileName = "Item Special", menuName = "Core/InventoryAPI/ItemSpecial")]
    public class ItemSpecial : Item
    {
        [field: SerializeField] public bool IsEventItem { get; set; }
    }
}