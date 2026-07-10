using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Ammo;
using Core.Scripts.Runtime.Combat;
using Core.Scripts.Runtime.Utilities;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities.StateMachine
{
    public class Entity_WeaponThrow : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody Rigidbody { get; set; }
        [field: SerializeField] public Transform WeaponThrowVisual { get; set; }
        [field: SerializeField] public Vector3 ThrowDirection { get; set; }

        private Transform _target;
        private float _throwSpeed;
        private float _throwRotationSpeed;
        private float _timer = 1f;
        private int _damage;

        /// <summary>Like bullets, only the server's copy hurts anybody.</summary>
        public bool IsAuthoritative { get; private set; }

        private void Update()
        {
            WeaponThrowVisual.Rotate(Vector3.right * (_throwRotationSpeed * Time.deltaTime));
            _timer -= Time.deltaTime;

            // The target can despawn mid-flight, when a player dies or disconnects.
            if(_timer > 0 && _target != null)
                ThrowDirection = _target.position + Vector3.up - transform.position;

            Rigidbody.linearVelocity = ThrowDirection.normalized * _throwSpeed;
            transform.forward = Rigidbody.linearVelocity;
        }

        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<Bullet>();
            var agent = other.GetComponent<Agent>();

            if (bullet == null && agent == null) return;

            if (IsAuthoritative && agent != null)
            {
                IDamageable damageable = agent.GetComponent<IDamageable>();

                if (damageable != null && damageable.IsAlive)
                    damageable.TakeDamage(_damage, ThrowDirection.normalized * _throwSpeed, transform.position);
            }

            var impactFx = GlobalPoolContainer.Instance.WeaponThrowImpactFx.GetObject();
            impactFx.transform.position = transform.position;
            GlobalPoolContainer.Instance.WeaponThrow.ReturnObject(this);
            GlobalPoolContainer.Instance.WeaponThrowImpactFx.ReturnObject(impactFx, 1f);
        }

        public void WeaponThrowSetup(float throwSpeed, Transform target, float timer, bool isAuthoritative, int damage)
        {
            _throwRotationSpeed = 1600f;

            _throwSpeed = throwSpeed;
            _target = target;
            _timer = timer;
            _damage = damage;
            IsAuthoritative = isAuthoritative;
        }
    }
}
