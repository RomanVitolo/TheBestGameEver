using System;
using System.Collections.Generic;
using Modules.InventoryAPI.Runtime;
using UnityEngine;

namespace Core.Scripts.Runtime.InventoryAPI.Runtime
{
    public class ItemSpawnerController : MonoBehaviour
    {
        private int[] _ratio = new int[Enum.GetValues(typeof(Item.FrequencyEnum)).Length];
        [SerializeField] public List<LootData> LooDataList = new List<LootData>();
        [SerializeField] public List<Item> ItemList = new List<Item>();
        [SerializeField] public List<Item> ItemListFrequency1  = new List<Item>();
        [SerializeField] public List<Item> ItemListFrequency5 = new List<Item>();
        [SerializeField] public List<Item> ItemListFrequency10 = new List<Item>();
        [SerializeField] public List<Item> ItemListFrequency25 = new List<Item>();
        [SerializeField] public List<Item> ItemListFrequency50 = new List<Item>();

        [SerializeField] public List<List<Item>> ItemListFrequency =
            new List<List<Item>>(Enum.GetValues(typeof(Item.FrequencyEnum)).Length);

        private void Start()
        {
            throw new NotImplementedException();
        }

        private void GetRatio()
        {
            int index = 0;
            foreach (Item.FrequencyEnum frequencyType  in Enum.GetValues(typeof(Item.FrequencyEnum)))
            {
                _ratio[index] = (int)frequencyType;
                index++;
            }
        }

        private void DivideList()
        {
            ItemListFrequency.Add(ItemListFrequency1);
            ItemListFrequency.Add(ItemListFrequency5);
            ItemListFrequency.Add(ItemListFrequency10);
            ItemListFrequency.Add(ItemListFrequency25);
            ItemListFrequency.Add(ItemListFrequency50);

            foreach (var item in ItemList)
            {
                switch (item.Frequency)
                {
                    case Item.FrequencyEnum.One:
                    {
                        ItemListFrequency1.Add(item);
                        break;
                    }
                    case Item.FrequencyEnum.Five:
                    {
                        ItemListFrequency5.Add(item);
                        break;
                    }
                    case Item.FrequencyEnum.Ten:
                    {
                        ItemListFrequency10.Add(item);
                        break;
                    }
                    case Item.FrequencyEnum.TwentyFive:
                    {
                        ItemListFrequency25.Add(item);
                        break;
                    }
                    case Item.FrequencyEnum.Fifty:
                    {
                        ItemListFrequency50.Add(item);
                        break;
                    }
                }
            }
        }

        private void Spawn()
        {
            
        }

        private void SearchEmptyPlaceInMatrix()
        {
            
        }
        
    }
}