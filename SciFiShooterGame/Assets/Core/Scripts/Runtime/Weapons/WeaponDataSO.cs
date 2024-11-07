using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public enum WeaponType
    {
        None = 0,
        Pistol = 1,
        SubMachine = 2,
        Rifle = 3,
        Sniper = 4,
        Shotgun = 5,
        Axe = 6,
        Hammer = 7,
        Sword = 8
    }

    public enum FireModeType
    {
        Single,
        Burst,
        Auto
    }

    public enum EquipType
    {
       SideGrab,
       BackGrab
    }

    public enum WeaponAnimationLayerType
    {    
        CommonHold = 1,
        LowHold = 2,
        HighHold = 3,
        MeleeHold = 4,
    }
    
    [CreateAssetMenu(menuName = "Core/Create AgentWeapon", fileName = "AgentWeapon")]
    public class WeaponDataSO : ScriptableObject
    {
        [field: SerializeField, Header("Weapon Settings")] public WeaponType WeaponType { get; private set; }
        [field: SerializeField, Header("Weapon Settings")] public FireModeType FireMode { get; private set; }
        [field: SerializeField] public int WeaponInputSlot { get; set; } 
        [field: SerializeField] public float WeaponFireRate { get; set; }
        private float _lastShootTime;
        [field: SerializeField] public int WeaponDurability { get; set; }
        [field: SerializeField, Range(1,5)] public float WeaponReloadSpeed { get; private set; }
        [field: SerializeField, Range(1,5)] public float WeaponEquipmentSpeed { get; private set; }
        [field: SerializeField, Header("Animation Layer")] public WeaponAnimationLayerType AnimationLayer { get; private set; }     
        [field: SerializeField] public EquipType EquipType { get; private set; }
        [field: SerializeField, Header("Ammo Settings")] public float BulletMass { get; private set; }     
        [field: SerializeField] public float BulletVelocity { get; set; } 
        [field: SerializeField] public int AmmoInMagazine { get; set; }
        [field: SerializeField] public int MagazineCapacity { get; set; }
        [field: SerializeField] public int TotalReserveAmmo { get; private set; }
        [field: SerializeField] public int InitialWeaponAmmo { get; private set; }
        
        public Transform GunPoint { get; set; }

        public void InitializeAmmo()
        {
            _lastShootTime = 0;
            TotalReserveAmmo = InitialWeaponAmmo;
            if(InitialWeaponAmmo > MagazineCapacity)
                AmmoInMagazine = MagazineCapacity;
        }

        public bool CanShoot()
        {
            if (HaveEnoughBullets() && ReadyToFire())
            {
                AmmoInMagazine--;
                return true;
            }
            return false;
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
            if (Time.time > _lastShootTime + 1 / WeaponFireRate)
            {
                _lastShootTime = Time.time;
                return true;
            }
            return false;
        }

        public void RefillAmmo()
        {
            //TotalReserveAmmo += AmmoInMagazine;
            
            int ammoToReload = MagazineCapacity;

            if (ammoToReload > TotalReserveAmmo)
                ammoToReload = TotalReserveAmmo;
            
            TotalReserveAmmo -= ammoToReload;
            AmmoInMagazine = ammoToReload;

            if (TotalReserveAmmo < 0)
                TotalReserveAmmo = 0;           
        }         
    }
}