using System;
using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities
{
    public class EntityAnimationEvents : MonoBehaviour
    {
        private Entity _entity;

        private void Awake()
        {
            _entity = GetComponentInParent<Entity>();
        }

        public void AnimationTrigger() => _entity.AnimationTrigger();

        public void StartManualMovement() => _entity.ActivateManualMovement(true);
        public void StopManualMovement() => _entity.ActivateManualMovement(false);
        public void StartManualRotation() => _entity.ActivateManualRotation(true);
        public void StopManualRotation() => _entity.ActivateManualRotation(false);
    }
}