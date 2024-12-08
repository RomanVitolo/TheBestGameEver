using System;

namespace Core.Scripts.Runtime.Utilities
{
    internal interface NotifyEvent
    {
        public event Action NotifyAction;
    }
}