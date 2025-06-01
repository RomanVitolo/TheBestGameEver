using System;
using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [Serializable]
    public class ItemData
    {
        public Item Item;
        public bool IsRotated = false;
        public Vector2Int MatrixPosition;
        private Vector3 slotPosition = Vector3.zero;
        //panelType
        public Panel.PanelTypeEnum SlotPanelType;
        public GameObject LootContainer;
        public int LootContainerId;
    }
}