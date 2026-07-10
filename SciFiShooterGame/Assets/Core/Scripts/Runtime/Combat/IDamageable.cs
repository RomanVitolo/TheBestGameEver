using UnityEngine;

namespace Core.Scripts.Runtime.Combat
{
    /// <summary>
    /// Anything a bullet or an enemy attack can hurt. Implementations must ignore calls made off the server:
    /// damage is server-authoritative, and clients only ever see the replicated result.
    /// </summary>
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(int amount, Vector3 force, Vector3 hitPoint);
    }
}
