using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
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

        /// <summary>Kicks the bone nearest the hit so the corpse falls away from the shot.</summary>
        public void ApplyImpulse(Vector3 force, Vector3 hitPoint)
        {
            Rigidbody nearest = null;
            float nearestSqr = float.MaxValue;

            foreach (var ragdollRigidbody in _ragdollRigidbodies)
            {
                float sqr = (ragdollRigidbody.worldCenterOfMass - hitPoint).sqrMagnitude;
                if (sqr >= nearestSqr) continue;

                nearestSqr = sqr;
                nearest = ragdollRigidbody;
            }

            if (nearest != null)
                nearest.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        }
        
    }
}