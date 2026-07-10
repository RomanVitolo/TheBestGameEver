using System.Linq;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    /// <summary>
    /// Mutable per-instance weapon state, previously stored on the shared <see cref="WeaponDataSO"/> asset.
    /// Two agents holding the same weapon type now have independent magazines, recoil and gun points.
    /// Seeded from the asset's authored defaults; the asset itself is never written to at runtime.
    /// </summary>
    public class WeaponRuntime
    {
        private const float const_RecoilCoolDown = 1f;

        private readonly WeaponDataSO _config;

        private float _currentRecoil;
        private float _lastRecoilUpdateTime;
        private float _lastShootTime;

        public WeaponDataSO Config => _config;
        public Transform GunPoint { get; }
        public int AmmoInMagazine { get; private set; }
        public int TotalReserveAmmo { get; private set; }
        public WeaponEnums.FireModeType FireMode { get; set; }

        public WeaponRuntime(WeaponDataSO config, Transform gunPoint)
        {
            _config = config;
            GunPoint = gunPoint;
            FireMode = config.FireMode;
            _currentRecoil = config.BaseRecoil;
            ResetAmmo();
        }

        /// <summary>Mirrors the old WeaponDataSO.InitializeAmmo behaviour, including its clamp.</summary>
        public void ResetAmmo()
        {
            _lastShootTime = 0f;
            TotalReserveAmmo = _config.InitialWeaponAmmo;
            AmmoInMagazine = _config.InitialWeaponAmmo > _config.MagazineCapacity
                ? _config.MagazineCapacity
                : _config.AmmoInMagazine;
        }

        public bool ReadyToShoot() => HaveEnoughBullets() && ReadyToFire();

        public bool HaveEnoughBullets() => AmmoInMagazine > 0;

        public void ConsumeBullet() => AmmoInMagazine--;

        public void AddReserveAmmo(int amount) => TotalReserveAmmo += amount;

        public bool CanReload()
        {
            if (AmmoInMagazine == _config.MagazineCapacity) return false;

            return TotalReserveAmmo > 0;
        }

        public void RefillAmmo()
        {
            int ammoToReload = _config.MagazineCapacity;

            if (ammoToReload > TotalReserveAmmo)
                ammoToReload = TotalReserveAmmo;

            TotalReserveAmmo -= ammoToReload;
            AmmoInMagazine = ammoToReload;

            if (TotalReserveAmmo < 0)
                TotalReserveAmmo = 0;
        }

        public bool HasThisWeaponFireMode()
        {
            bool isActiveOrNot = false;
            foreach (var fireMode in _config.WeaponFireMode.FireModeTypesList)
            {
                isActiveOrNot = fireMode.HasThisModeAvailable;
            }

            return isActiveOrNot;
        }

        public void CycleFireMode(int fireModeIndex) =>
            FireMode = _config.WeaponFireMode.FireModeTypesList[fireModeIndex].FireModeType;

        public Vector3 ApplyRecoil(Vector3 originalDirection)
        {
            UpdateRecoil();

            float randomizedValue = Random.Range(-_currentRecoil, _currentRecoil);
            Quaternion recoilRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);

            return recoilRotation * originalDirection;
        }

        private bool ReadyToFire()
        {
            foreach (var weaponFireRate in _config.WeaponFireMode.FireModeTypesList
                         .Where(weaponFireRate => weaponFireRate.FireModeType == FireMode)
                         .Where(weaponFireRate => Time.time > _lastShootTime + 1 / weaponFireRate.WeaponFireRate))
            {
                _lastShootTime = Time.time;
                return true;
            }

            return false;
        }

        private void UpdateRecoil()
        {
            if (Time.time > _lastRecoilUpdateTime + const_RecoilCoolDown)
                _currentRecoil = _config.BaseRecoil;
            else
                IncreaseRecoil();

            _lastRecoilUpdateTime = Time.time;
        }

        private void IncreaseRecoil() => _currentRecoil =
            Mathf.Clamp(_currentRecoil + _config.RecoilIncreaseRate, _config.BaseRecoil, _config.MaximumRecoil);
    }
}
