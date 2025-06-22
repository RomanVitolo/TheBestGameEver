using System.Collections;
using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class BaseObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected string _poolName;
        [SerializeField] protected T _prefabType; 
        [SerializeField] protected int _initialPoolSize = 10;
        [SerializeField] protected Transform _objectParent;
        
        protected ObjectPool<T> objectPool;

        protected virtual void Start()
        {
            if (_objectParent != null) return;
            GameObject parentObject = new GameObject(_poolName);
            _objectParent = parentObject.transform;
        }

        public T GetObject()
        {
            return objectPool.Get();
        }
        
        public virtual void ReturnObject(T objectToReturn)
        {
            objectPool.ReturnToPool(objectToReturn);
        }
      
        public void ReturnObject(T objectToReturn, float delay)
        {
            if (delay <= 0f)
                objectPool.ReturnToPool(objectToReturn);
            else
                StartCoroutine(ReturnToPoolAfterDelay(objectToReturn, delay));
        }

        private IEnumerator ReturnToPoolAfterDelay(T obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            objectPool.ReturnToPool(obj);
        }
        
    }
}