using UnityEngine;

namespace Core.Scripts.Runtime.Movement.Interfaces
{
    public interface IAimHandler
    {
        void HandleAim(Vector3 movementValue, RaycastHit mouseHitInfo);
    }
}