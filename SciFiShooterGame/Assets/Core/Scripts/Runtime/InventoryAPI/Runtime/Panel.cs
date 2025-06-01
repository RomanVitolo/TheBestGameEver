using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    [Serializable]
    public class Panel : MonoBehaviour
    {
        [field: SerializeField] public PanelTypeEnum PanelType { get; set; }
        [field: SerializeField,Header("X: Row, Y: Column")] public Vector2Int Size { get; set; }
        [field: SerializeField] public bool[,] Matrix { get; set; } = new bool[1, 1];
        [field: SerializeField] public List<GameObject> ItemObjectList { get; set; } = new List<GameObject>();
        [field: SerializeField] public List<ItemData> ItemDataList { get; set; } = new List<ItemData>();
        [field: SerializeField] public Transform SlotParent { get; set; }
        [field: SerializeField] public Transform ItemParent { get; set; }
        
        public enum PanelTypeEnum
        {
            Character,
            BackPack,
            Loot
        }
    }
}