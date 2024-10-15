using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance;

        [SerializeField] private GameObject _prefabType;
        [SerializeField] private int _poolSize = 10;

        private Queue<GameObject> gameObjectsQueue;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            gameObjectsQueue = new Queue<GameObject>();
            CreateInitialPool();
        }

        public GameObject GetObject()
        {
            if (gameObjectsQueue.Count == 0)
                CreateObject();
            
            GameObject objectToGet = gameObjectsQueue.Dequeue();
            objectToGet.SetActive(true);
            objectToGet.transform.parent = null;

            return objectToGet;
        }

        public void ReturnObjectToPool(GameObject objectToReturn)
        {
            objectToReturn.SetActive(false);
            gameObjectsQueue.Enqueue(objectToReturn);
            objectToReturn.transform.parent = transform;
        }

        private void CreateInitialPool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                CreateObject();
            }
        }

        private void CreateObject()
        {
            GameObject newGameObject = Instantiate(_prefabType, transform);
            newGameObject.SetActive(false);
            gameObjectsQueue.Enqueue(newGameObject);
        }
    }
}
