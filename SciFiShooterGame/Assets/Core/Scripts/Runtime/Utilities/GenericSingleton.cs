using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class GenericSingleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _applicationIsQuitting = false;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance of " + typeof(T) + " already destroyed. Returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = CreateOrAssignInstance();
                    }

                    return _instance;
                }
            }
        }

        private static T CreateOrAssignInstance()
        {
            T existingInstance = GameObject.FindFirstObjectByType<T>();

            if (existingInstance != null)
            {
                return existingInstance;
            }

            GameObject singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
            T newInstance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);

            return newInstance;
        }

        protected virtual void OnDestroy()
        {
            _applicationIsQuitting = true;
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }
}
