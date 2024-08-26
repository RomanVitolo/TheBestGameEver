using UnityEngine;

namespace Core.Scripts.Runtime.Weapon
{
    public class WeaponAnimations
    {
        public readonly int Reload = Animator.StringToHash("Reload");
        public readonly int WeaponGrabType = Animator.StringToHash("WeaponGrabType");
        public readonly int WeaponGrab = Animator.StringToHash("WeaponGrab");
        public readonly int BusyGrabbingWeapon = Animator.StringToHash("BusyGrabbingWeapon");
    }
}
