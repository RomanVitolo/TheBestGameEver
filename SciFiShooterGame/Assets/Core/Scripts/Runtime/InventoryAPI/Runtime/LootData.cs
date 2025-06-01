using System;
using System.Collections.Generic;
using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    public class LootData : MonoBehaviour
    {
        [field: SerializeField] public string Id { get; private set; } = Guid.NewGuid().ToString();
        [field: SerializeField] public Vector2Int Size { get; private set; }

        [field: SerializeField] public int[] FrequencyCount { get; private set; } =
            new int[Enum.GetValues(typeof(Item.FrequencyEnum)).Length];
        [field: SerializeField] public int[] MaxFrequencyCount { get; private set; } = 
            new int[Enum.GetValues(typeof(Item.FrequencyEnum)).Length];
        
        [field: SerializeField] public List<ItemData> ItemList { get; private set; } = new List<ItemData>();
        [field: SerializeField] public bool IsFull { get; set; } = false;
        public bool[,] Matrix { get; set; }

        private void Awake()
        {
            Matrix = new bool[Size.x, Size.y];
        }
    }
}