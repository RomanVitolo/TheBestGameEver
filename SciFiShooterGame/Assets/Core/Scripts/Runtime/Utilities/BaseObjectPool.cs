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
        
    }
}