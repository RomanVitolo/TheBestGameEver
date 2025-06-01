using System;
using UnityEngine;
using UnityEngine.Events;

namespace CombatDesign.GlobalTriggers.Runtime
{
    [CreateAssetMenu(fileName = "GlobalTriggerEvent", menuName = "GlobalTriggerEvent")]
    public class GlobalTriggerEvent : ScriptableObject
    {
        public UnityEvent ReleaseEvent;

        public void TriggerCurrentEvent()
        {
            ReleaseEvent?.Invoke();
        }

        private void OnDestroy()
        {
            ReleaseEvent.RemoveAllListeners();
        }
    }
}