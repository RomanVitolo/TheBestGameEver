using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    
    [CreateAssetMenu(menuName = "Core/Weapon Settings/Create AgentWeapon", fileName = "AgentWeapon")]
    [InlineEditor]
    public class WeaponDataSO : SerializedScriptableObject
    {
        [field: SerializeField] public string WeaponName { get; set; }
        [field: SerializeField] public float CameraDistance { get; set; }
        public Transform GunPoint { get; set; }
        [field: SerializeField, BoxGroup("Weapon Settings")] public WeaponEnums.WeaponType WeaponType { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Settings")] public WeaponEnums.FireModeType FireMode { get; set; }
        [field: SerializeField, BoxGroup("Weapon Settings")] public int WeaponInputSlot { get; set; }
        [field: SerializeField, Range(2,12), BoxGroup("Weapon Settings")] public float WeaponDistance { get; set; }
        [field: SerializeField, BoxGroup("Weapon Settings"), Range(0,100)] public int WeaponDurability { get; set; }
        [field: SerializeField, Range(1,5), BoxGroup("Weapon Settings")] public float WeaponReloadSpeed { get; private set; }
        [field: SerializeField, Range(1,5), BoxGroup("Weapon Settings")] public float WeaponEquipmentSpeed { get; private set; }
        [field: SerializeField, BoxGroup("Animation Layer Settings")] public WeaponEnums.WeaponAnimationLayerType AnimationLayer { get; private set; }     
        [field: SerializeField, BoxGroup("Animation Layer Settings")] public WeaponEnums.EquipType EquipType { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Fire Mode Data"), InlineEditor]
        public WeaponFireModeHolderSO WeaponFireMode { get; private set; }
        
        [field: SerializeField, BoxGroup("Ammo Settings"), PreviewField(100), HideLabel] 
        public GameObject BulletPrefab { get; private set; }
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(0.3f,0.5f,1f)] 
        public WeaponEnums.WeaponAmmoType AmmoType { get; private set; }
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(0.8f,0.4f,0.4f)] 
        public float BulletMass { get; private set; }     
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(1f,1f,0f)] 
        public float BulletVelocity { get; set; } 
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(1f,1f,0f)] 
        public float BulletImpactForce { get; set; }
        
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int AmmoInMagazine { get; set; }
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int MagazineCapacity { get; set; }
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int TotalReserveAmmo { get;  set; }
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int InitialWeaponAmmo { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Recoil")] public float BaseRecoil { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Recoil")] public float MaximumRecoil { get; set; }
        [field: SerializeField, BoxGroup("Weapon Recoil"), Range(0, 1)] public float RecoilIncreaseRate { get; set; }
        private float _currentRecoil = 1f;
        private float _lastRecoilUpdateTime;
        private const float const_RecoilCoolDown = 1f;
        private float _lastShootTime;
        public void InitializeAmmo()
        {
            TotalReserveAmmo = 0;
            _lastShootTime = 0;
            TotalReserveAmmo = InitialWeaponAmmo;
            if(InitialWeaponAmmo > MagazineCapacity)
                AmmoInMagazine = MagazineCapacity;
        }

        public bool ReadyToShoot()
        {
            if (!HaveEnoughBullets() || !ReadyToFire()) return false;
            //AmmoInMagazine--;
            return true;
        }
        
        [Button(ButtonSizes.Large), GUIColor(1f,1f,0f)]
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

        public bool HasThisWeaponFireMode()
        {
            bool isActiveOrNot = false;
            foreach (var fireMode in WeaponFireMode.FireModeTypesList)
            {
                isActiveOrNot = fireMode.HasThisModeAvailable;
            }

            return isActiveOrNot;
        }
    }
}