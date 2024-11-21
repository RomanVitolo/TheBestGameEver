using NUnit.Framework;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    [CreateAssetMenu(menuName = "Core/Weapon Settings/Create FireModeSettings", fileName = "WeaponFireModeType")]
    public class WeaponFireModeS0 : WeaponFireModeType
    {
        [field: SerializeField] public bool HasThisModeAvailable { get; private set; }
        [field: SerializeField] public bool FireModeActive{ get; set; }
        [field: SerializeField] public WeaponEnums.FireModeType FireModeType { get; private set; }

        public bool BurstModeEnabled() => FireModeActive;

        public float BurstModeDelay() => fireDelay;
        public int BulletsPerShotInBurstMode() => bulletsPerShot;

        public float WeaponFireRate => fireRate;
     
        
        public void ToggleBurstMode()
        {
            if (!HasThisModeAvailable) return;

            FireModeActive = !FireModeActive;
        }
    }
}