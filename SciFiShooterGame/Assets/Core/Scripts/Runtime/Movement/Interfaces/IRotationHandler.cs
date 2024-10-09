using UnityEngine;

namespace Core.Scripts.Runtime.Movement.Interfaces
{
    public interface IRotationHandler
    {
        void ApplyRotation(RaycastHit mouseHitInfo);
    }
}