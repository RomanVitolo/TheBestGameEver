using Core.Scripts.Runtime.AI.Entities.StateMachine;
using Core.Scripts.Runtime.Combat;
using Core.Scripts.Runtime.Utilities;
using UnityEngine;

namespace Core.Scripts.Runtime.Ammo
{
    public class Bullet : MonoBehaviour
    {
        public float ImpactForce;

        [SerializeField] private GameObject bulletImpactEffect;
        [SerializeField] private int _damage = 1;

        private Rigidbody _rigidbody => GetComponent<Rigidbody>();
        private TrailRenderer _trailRenderer => GetComponent<TrailRenderer>();
        private BoxCollider _collider => GetComponent<BoxCollider>();
        private MeshRenderer _bulletMeshRenderer => GetComponent<MeshRenderer>();

        private bool _bulletDisabled;

        /// <summary>Only the server's copy of a shot deals damage; the others exist to be seen.</summary>
        public bool IsAuthoritative { get; private set; }

        private void OnCollisionEnter(Collision other)
        {
            InstantiateImpactEffect(other);

            Vector3 travelDirection = _rigidbody.linearVelocity.normalized;
            bool wasAuthoritative = IsAuthoritative;
            bool hasContact = other.contacts.Length > 0;
            Vector3 contactPoint = hasContact ? other.contacts[0].point : transform.position;

            GlobalPoolContainer.Instance.BulletPool.ReturnObject(this);

            if (!wasAuthoritative || !hasContact) return;

            Entity_Shield shield = other.gameObject.GetComponent<Entity_Shield>();

            if (shield != null)
            {
                shield.ReduceDurability();
                return;
            }

            IDamageable damageable = other.gameObject.GetComponentInParent<IDamageable>();

            if (damageable == null || !damageable.IsAlive) return;

            damageable.TakeDamage(_damage, travelDirection * ImpactForce, contactPoint);
        }

        private Vector3 _startPosition;
        private float _flyDistance;

        public void BulletSetup(float flyDistance, float impactForce, bool isAuthoritative)
        {
            ImpactForce = impactForce;
            IsAuthoritative = isAuthoritative;

            _bulletDisabled = false;
            _collider.enabled = true;
            _trailRenderer.enabled = true;
            _bulletMeshRenderer.enabled = true;

            _trailRenderer.time = 0.25f;
            _startPosition = transform.position;
            _flyDistance = flyDistance + 1f;
        }

        private void Update()
        {
            FaceTrailIfNeeded();

            CheckIfShouldBeDisabled();

            if(_trailRenderer.time < 0)
                GlobalPoolContainer.Instance.BulletPool.ReturnObject(this);
        }

        private void CheckIfShouldBeDisabled()
        {
            if (Vector3.Distance(_startPosition, transform.position) > _flyDistance && !_bulletDisabled)
            {
                _collider.enabled = false;
                _bulletMeshRenderer.enabled = false;
                _bulletDisabled = true;
            }
        }

        private void FaceTrailIfNeeded()
        {
            if (Vector3.Distance(_startPosition, transform.position) > _flyDistance - 1.5f)
                _trailRenderer.time -= 2f * Time.deltaTime;
        }

        private void InstantiateImpactEffect(Collision other)
        {
            if (other.contacts.Length <= 0) return;

            ContactPoint contact = other.contacts[0];

            var impact = GlobalPoolContainer.Instance.BulletPoolImpact.GetObject();
            impact.transform.SetPositionAndRotation(contact.point, Quaternion.LookRotation(contact.normal));
        }
    }
}
