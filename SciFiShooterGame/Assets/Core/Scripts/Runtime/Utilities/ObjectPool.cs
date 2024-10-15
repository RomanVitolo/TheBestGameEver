using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Queue<T> objects = new Queue<T>();
        private readonly Transform parentTransform; 
        
        public ObjectPool(T prefab, int initialSize, Transform parent)
        {
            this.prefab = prefab;
            this.parentTransform = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T newObject = Object.Instantiate(prefab, parent); 
                newObject.gameObject.SetActive(false);
                objects.Enqueue(newObject);
            }
        }
        
        public T Get()
        {
            if (objects.Count > 0)
            {
                T obj = objects.Dequeue();
                obj.gameObject.SetActive(true);
                obj.transform.SetParent(parentTransform); 
                return obj;
            }
            else
            {
                T newObject = Object.Instantiate(prefab, parentTransform);  
                return newObject;
            }
        }
       
        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(parentTransform);  
            objects.Enqueue(obj);
        }
    }
}
