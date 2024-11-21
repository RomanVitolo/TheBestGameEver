using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    [CreateAssetMenu(menuName = "Core/Weapon Settings/Create FireModeHolder", fileName = "FireModeHolder")]
    public class WeaponFireModeHolderSO : ScriptableObject
    {
        [field: SerializeField] public List<int> FireModeTypes { get; set; } = new List<int>();
    }
    
    
    [CreateAssetMenu(menuName = "Core/Weapon Settings/Create BurstModeSettings", fileName = "BurstMode")]
    public class WeaponFireModeSO : WeaponFireModeType
    {
        [field: SerializeField] public bool BurstModeAvailable { get; private set; }
        [field: SerializeField] public bool BurstModeActive{ get; set; }

        public bool BurstModeEnabled() => BurstModeActive;

        public float BurstModeDelay() => fireDelay;
        public int BulletsPerShotInBurstMode() => bulletsPerShot;
     
        
        public void ToggleBurstMode()
        {
            if (!BurstModeAvailable) return;

            BurstModeActive = !BurstModeActive;
        }
    }
}