using UnityEngine;

namespace Core.Scripts.Runtime.Ammo
{
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _rigidbody => GetComponent<Rigidbody>();
        private void OnCollisionEnter(Collision other)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}