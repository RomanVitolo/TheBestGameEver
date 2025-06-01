using Core.Scripts.Runtime.Utilities;
using UnityEngine;

namespace Core.Scripts.Runtime.Ammo
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private GameObject bulletImpactEffect;
        
        private Rigidbody _rigidbody => GetComponent<Rigidbody>();
        private TrailRenderer _trailRenderer => GetComponent<TrailRenderer>();
        private BoxCollider _collider => GetComponent<BoxCollider>();
        private MeshRenderer _bulletMeshRenderer => GetComponent<MeshRenderer>();

        private bool _bulletDisabled;

        private void OnCollisionEnter(Collision other)
        {
            InstantiateImpactEffect(other);
            GlobalPoolContainer.Instance.BulletPool.ReturnObject(this);
        }
        
        private Vector3 _startPosition;
        private float _flyDistance;

        public void BulletSetup(float flyDistance)
        {
            _bulletDisabled = false;
            _collider.enabled = true;
            _trailRenderer.enabled = true;
            
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