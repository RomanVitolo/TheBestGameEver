using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class ObjectPool<T> where T : Component
    {
        private T prefab;
        private Queue<T> objects = new Queue<T>();
        private Transform parentTransform;  // Parent transform to store the pooled objects

        // Constructor to initialize the pool with a parent for the objects
        public ObjectPool(T prefab, int initialSize, Transform parent)
        {
            this.prefab = prefab;
            this.parentTransform = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T newObject = GameObject.Instantiate(prefab, parent); // Instantiate under the parent
                newObject.gameObject.SetActive(false);
                objects.Enqueue(newObject);
            }
        }

        // Get an object from the pool
        public T Get()
        {
            if (objects.Count > 0)
            {
                T obj = objects.Dequeue();
                obj.gameObject.SetActive(true);
                obj.transform.SetParent(parentTransform);  // Ensure object is under the parent
                return obj;
            }
            else
            {
                T newObject = GameObject.Instantiate(prefab, parentTransform);  // Create new object under parent
                return newObject;
            }
        }

        // Return an object to the pool
        public void ReturnToPool(T obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(parentTransform);  // Make sure it stays under the parent
            objects.Enqueue(obj);
        }
    }
}
