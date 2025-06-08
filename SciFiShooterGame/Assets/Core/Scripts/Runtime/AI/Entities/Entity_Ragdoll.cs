using System;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities
{
    public class Entity_Ragdoll : MonoBehaviour
    {
        [SerializeField] private Transform _ragdollParent;
        [SerializeField] private Collider[] _ragdollColliders;
        [SerializeField] private Rigidbody[] _ragdollRigidbodies;

        private void Awake()
        {
            _ragdollColliders = GetComponentsInChildren<Collider>();
            _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            
            RagdollActive(false);
        }

        public void RagdollActive(bool active)
        {
            foreach (var _ragdollRigidbody in _ragdollRigidbodies)
            {
                _ragdollRigidbody.isKinematic = !active;
            }
        }

        public void CollidersActive(bool active)
        {
            foreach (var _ragdollCollider in _ragdollColliders)
            {
                _ragdollCollider.enabled = active;
            }
        }
        
    }
}