using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public class WeaponAnimations
    {
        public readonly int Reload = Animator.StringToHash("Reload");
        public readonly int WeaponReloadSpeed = Animator.StringToHash("ReloadSpeed");
        public readonly int EquipType = Animator.StringToHash("EquipType");
        public readonly int EquipWeapon = Animator.StringToHash("EquipWeapon");
        public readonly int BusyEquippingWeapon = Animator.StringToHash("BusyEquippingWeapon");
        public readonly int Fire = Animator.StringToHash("Fire");
        public readonly int EquipSpeed = Animator.StringToHash("EquipSpeed");
    }
}
