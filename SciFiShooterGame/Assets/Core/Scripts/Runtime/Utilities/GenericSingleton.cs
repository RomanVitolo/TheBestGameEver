using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class GenericSingleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new();

        public static T Instance
        {
            get
            {
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
            T existingInstance = FindFirstObjectByType<T>();

            if (existingInstance != null)
            {
                return existingInstance;
            }

            GameObject singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
            T newInstance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);

            return newInstance;
        }
       
    }
}
