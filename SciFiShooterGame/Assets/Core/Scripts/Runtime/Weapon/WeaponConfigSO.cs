using UnityEngine;

namespace Core.Scripts.Runtime.Weapon
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

    public enum GrabType
    {
       SideGrab,
       BackGrab
    }  
    
    [CreateAssetMenu(menuName = "Core/Create AgentWeapon", fileName = "AgentWeapon")]
    public class WeaponConfigSO : ScriptableObject
    {
        [field: SerializeField, Header("Weapon Settings")] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public int WeaponInputSlot { get; set; } 
        [field: SerializeField] public float WeaponFireRate { get; set; }
        [field: SerializeField] public int WeaponDurability { get; set; }
        [field: SerializeField, Header("Animation Layer")] public int AnimationLayer { get; private set; }     
        [field: SerializeField] public GrabType GrabType { get; private set; }
        [field: SerializeField, Header("Ammo Settings")] public float BulletMass { get; private set; }     
        [field: SerializeField] public float BulletVelocity { get; set; } 
        [field: SerializeField] public int CurrentAmmo { get; set; }
        [field: SerializeField] public int MaxWeaponAmmo { get; set; }
    }
}