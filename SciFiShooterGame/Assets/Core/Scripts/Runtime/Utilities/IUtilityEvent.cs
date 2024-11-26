using System;

namespace Core.Scripts.Runtime.Utilities
{
    internal interface IUtilityEvent
    {
        public event Action NotifyAction;
    }
}