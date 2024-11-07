using System;
using Core.Scripts.Runtime.Utilities;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private Transform _weaponGunPoint;
        
        [Header("Weapon Configuration")]
        [field: SerializeField] public WeaponDataSO WeaponDataConfiguration { get; private set; }

        private void Awake() =>  
            _weaponGunPoint ??= GetComponentInChildren<GunPointTransform>().transform;
        

        private void OnEnable()
        {
            WeaponDataConfiguration.GunPoint = _weaponGunPoint;
            WeaponDataConfiguration.InitializeAmmo();   
        }
    }
}