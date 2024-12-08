namespace Core.Scripts.Runtime.Weapons
{
    public abstract class WeaponEnums
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
            Auto,
            MultiShoot
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

        public enum WeaponAmmoType
        {
            PistolBullet,
            SubMachineBullet,
            ShotgunBullet,
            RifleBullet,
        }
    }
}