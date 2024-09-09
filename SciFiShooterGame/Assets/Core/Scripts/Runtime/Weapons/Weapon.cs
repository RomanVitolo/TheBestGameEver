using System;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [Header("Weapon Configuration")]
        [field: SerializeField] public WeaponDataSO WeaponDataConfiguration { get; private set; }

        private void OnEnable()
        {
            WeaponDataConfiguration.InitializeAmmo();
        }
    }
}