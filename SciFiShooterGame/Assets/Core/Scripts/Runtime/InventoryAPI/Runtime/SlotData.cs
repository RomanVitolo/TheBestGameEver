using UnityEngine;
using UnityEngine.EventSystems;

namespace Modules.InventoryAPI.Runtime
{
    public class SlotData : MonoBehaviour, IDropHandler, IPointerEnterHandler
    {
        [field: SerializeField] public Item.SlotTypeEnum Type { get; set; }
        [field: SerializeField] public Vector2Int MatrixPosition { get; set; }
        [field: SerializeField] public Panel.PanelTypeEnum PanelType { get; set; }
        [field: SerializeField] public GameObject MyLootContainer { get; set; }
        [field: SerializeField] public bool IsFull { get; set; }
        
        public void OnDrop(PointerEventData eventData)
        {
            
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
           
        }
    }
}