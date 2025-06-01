using System;
using UnityEngine;

namespace Modules.InventoryAPI.Runtime
{
    public abstract class Item : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set;}
        [field: SerializeField, Multiline] public string ItemDescription { get; private set;}
        [field: SerializeField] public string ItemID { get; private set;} = Guid.NewGuid().ToString();
        [field: SerializeField] public ItemTypeEnum ItemType { get; private set;}
        [field: SerializeField] public SlotTypeEnum SlotType { get; private set;}
        [field: SerializeField] public Vector2Int SlotSize { get; private set;}
        [field: SerializeField] public Sprite ItemImage { get; private set;}
        [field: SerializeField] public Color ItemColor { get; private set;}
        [field: SerializeField] public GameObject ItemPrefab { get; private set;}
        [field: SerializeField] public FrequencyEnum Frequency { get; set;}
        
        
        public enum ItemTypeEnum
        {
            None,
            Food,
            Cloth,
            Ammo,
            Weapon,
            Tool,
            Magazine,
            Special
        }

        public enum SlotTypeEnum
        {
            General,
            SpecialBackpack,
            Container,
            Tool,
            Machine,
        }

        public enum FrequencyEnum
        {
            One = 1,
            Five = 5,
            Ten = 10,
            TwentyFive = 25,
            Fifty = 50
        }
    }
}