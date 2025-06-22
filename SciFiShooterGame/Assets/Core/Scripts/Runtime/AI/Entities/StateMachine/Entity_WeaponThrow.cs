using System;
using System.Collections;
using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Ammo;
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

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

        private void Update()
        {
            WeaponThrowVisual.Rotate(Vector3.right * (_throwRotationSpeed * Time.deltaTime));
            _timer -= Time.deltaTime;
            
            if(_timer > 0)
                ThrowDirection = _target.position + Vector3.up - transform.position;
            
            Rigidbody.linearVelocity = ThrowDirection.normalized * _throwSpeed;
            transform.forward = Rigidbody.linearVelocity;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<Bullet>();
            var target = other.GetComponent<Agent>();

            if (bullet != null || target != null)
            {
                var impactFx = GlobalPoolContainer.Instance.WeaponThrowImpactFx.GetObject();
                impactFx.transform.position = transform.position;
                GlobalPoolContainer.Instance.WeaponThrow.ReturnObject(this);
                GlobalPoolContainer.Instance.WeaponThrowImpactFx.ReturnObject(impactFx, 1f);
            }
        }

        public void WeaponThrowSetup(float throwSpeed, Transform target, float timer)
        {
            _throwRotationSpeed = 1600f;
            
            _throwSpeed = throwSpeed;
            _target = target;
            _timer = timer;
        }
    }
}