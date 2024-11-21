using System.Linq;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    
    [CreateAssetMenu(menuName = "Core/Weapon Settings/Create AgentWeapon", fileName = "AgentWeapon")]
    public class WeaponDataSO : ScriptableObject
    {
        public Transform GunPoint { get; set; }
        [field: SerializeField, Header("Weapon Settings")] public WeaponEnums.WeaponType WeaponType { get; private set; }
        [field: SerializeField] public WeaponEnums.FireModeType FireMode { get; private set; }
        [field: SerializeField] public int WeaponInputSlot { get; set; }
        [field: SerializeField] public int WeaponDurability { get; set; }
        [field: SerializeField, Range(1,5)] public float WeaponReloadSpeed { get; private set; }
        [field: SerializeField, Range(1,5)] public float WeaponEquipmentSpeed { get; private set; }
        [field: SerializeField, Header("Animation Layer")] public WeaponEnums.WeaponAnimationLayerType AnimationLayer { get; private set; }     
        [field: SerializeField] public WeaponEnums.EquipType EquipType { get; private set; }
        [field: SerializeField, Header("Weapon Fire Mode Settings")] public WeaponFireModeHolderSO WeaponFireMode { get; private set; }
        [field: SerializeField, Header("Ammo Settings")] public float BulletMass { get; private set; }     
        [field: SerializeField] public float BulletVelocity { get; set; } 
        [field: SerializeField] public int AmmoInMagazine { get; set; }
        [field: SerializeField] public int MagazineCapacity { get; set; }
        [field: SerializeField] public int TotalReserveAmmo { get; private set; }
        [field: SerializeField] public int InitialWeaponAmmo { get; private set; }
        [field: SerializeField, Header("Weapon Recoil ")] public float BaseRecoil { get; private set; }
        [field: SerializeField] public float MaximumRecoil { get; set; }
        [field: SerializeField] public float RecoilIncreaseRate { get; set; }
        private float _currentRecoil = 1f;
        private float _lastRecoilUpdateTime;
        private const float const_RecoilCoolDown = 1f;
        

        private float _lastShootTime;
        public void InitializeAmmo()
        {
            _lastShootTime = 0;
            TotalReserveAmmo = InitialWeaponAmmo;
            if(InitialWeaponAmmo > MagazineCapacity)
                AmmoInMagazine = MagazineCapacity;
        }

        public bool ReadyToShoot()
        {
            if (!HaveEnoughBullets() || !ReadyToFire()) return false;
            AmmoInMagazine--;
            return true;
        }

        private bool HaveEnoughBullets()
        {
            if (AmmoInMagazine <= 0) return false;
            return true;
        }

        public bool CanReload()
        {
            if (AmmoInMagazine == MagazineCapacity) return false;
            
            return TotalReserveAmmo > 0; 
        }

        private bool ReadyToFire()
        {
            foreach (var weaponFireRate in WeaponFireMode.FireModeTypesList.
                         Where(weaponFireRate => weaponFireRate.FireModeType == FireMode).
                         Where(weaponFireRate => Time.time > _lastShootTime + 1 / weaponFireRate.WeaponFireRate))
            {
                _lastShootTime = Time.time; 
                return true;
            }

            return false;
        }
           

        public void RefillAmmo()
        {
            int ammoToReload = MagazineCapacity;

            if (ammoToReload > TotalReserveAmmo)
                ammoToReload = TotalReserveAmmo;
            
            TotalReserveAmmo -= ammoToReload;
            AmmoInMagazine = ammoToReload;

            if (TotalReserveAmmo < 0)
                TotalReserveAmmo = 0;           
        }         
        
        public Vector3 ApplyRecoil(Vector3 originalDirection)
        {
            UpdateRecoil();
            
            float randomizedValue = Random.Range(-_currentRecoil, _currentRecoil);
            
            Quaternion recoilRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
            
            return recoilRotation * originalDirection;
        }

        private void UpdateRecoil()
        {
            if (Time.time > _lastRecoilUpdateTime + const_RecoilCoolDown)
                _currentRecoil = BaseRecoil;
            else
             IncreaseRecoil();
            
            _lastRecoilUpdateTime = Time.time;
        }

        private void IncreaseRecoil() => _currentRecoil = 
            Mathf.Clamp(_currentRecoil + RecoilIncreaseRate, BaseRecoil, MaximumRecoil);
    }
}