using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public class WeaponAnimations
    {
        public readonly int Reload = Animator.StringToHash("Reload");
        public readonly int WeaponGrabType = Animator.StringToHash("WeaponGrabType");
        public readonly int WeaponGrab = Animator.StringToHash("WeaponGrab");
        public readonly int BusyGrabbingWeapon = Animator.StringToHash("BusyGrabbingWeapon");
        public readonly int Fire = Animator.StringToHash("Fire");
    }
}
