using Core.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Scripts.Runtime.Ammo
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private Bullet _bulletPrefab; // Prefab reference for bullets
        [SerializeField] private int _initialPoolSize = 10;
        [SerializeField] private Transform _bulletParent;
        private ObjectPool<Bullet> bulletPool; // Generic object pool for bullets

        private void Start()
        {
            // If no parent is assigned, create one dynamically
            if (_bulletParent == null)
            {
                GameObject bulletParentObject = new GameObject("BulletPool");
                _bulletParent = bulletParentObject.transform;
            }

            // Initialize the pool with the parent object
            bulletPool = new ObjectPool<Bullet>(_bulletPrefab, _initialPoolSize, _bulletParent);
        }

        // Get a bullet from the pool
        public Bullet GetBullet()
        {
            return bulletPool.Get();
        }

        // Return a bullet to the pool
        public void ReturnBullet(Bullet bullet)
        {
            bulletPool.ReturnToPool(bullet);
        }
    }
}