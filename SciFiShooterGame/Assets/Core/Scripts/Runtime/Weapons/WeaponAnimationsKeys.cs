﻿using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public class WeaponAnimationsKeys
    {
        public readonly int Reload = Animator.StringToHash("Reload");
        public readonly int WeaponReloadSpeed = Animator.StringToHash("ReloadSpeed");
        public readonly int EquipType = Animator.StringToHash("EquipType");
        public readonly int EquipWeapon = Animator.StringToHash("EquipWeapon");
        public readonly int Fire = Animator.StringToHash("Fire");
        public readonly int EquipSpeed = Animator.StringToHash("EquipSpeed");
    }
}
