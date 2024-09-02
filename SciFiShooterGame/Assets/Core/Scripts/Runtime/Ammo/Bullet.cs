using UnityEngine;

namespace Core.Scripts.Runtime.Ammo
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject bulletImpactEffect;
        
        private Rigidbody _rigidbody => GetComponent<Rigidbody>();
        private void OnCollisionEnter(Collision other)
        {
            InstantiateImpactEffect(other);      
            Destroy(gameObject);
        }

        private void InstantiateImpactEffect(Collision other)
        {
            if (other.contacts.Length > 0)
            {
                ContactPoint contact = other.contacts[0];
               
                GameObject newImpactEffect =
                    Instantiate(bulletImpactEffect, contact.point, Quaternion.LookRotation(contact.normal));
                
                Destroy(newImpactEffect, 1f);
            }
        }
    }
}