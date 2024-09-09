using System;

namespace Core.Scripts.Runtime.Utilities
{
    public interface UtilityEvent
    {
        public event Action NotifyAction;
    }
}