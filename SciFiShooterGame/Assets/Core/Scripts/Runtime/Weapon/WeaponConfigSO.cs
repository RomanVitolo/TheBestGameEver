using UnityEngine;

namespace Core.Scripts.Runtime.Weapon
{
    public enum WeaponType
    {
        None,
        Pistol,
        SubMachine,
        Rifle,
        Sniper,
        Shotgun,
        Axe,
        Hammer,
        Sword
    }

    public enum GrabType
    {
       SideGrab,
       BackGrab
    }  
    
    [CreateAssetMenu(menuName = "Core/Create AgentWeapon", fileName = "AgentWeapon")]
    public class WeaponConfigSO : ScriptableObject
    {
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public GrabType GrabType { get; private set; }
        [field: SerializeField] public int WeaponInputSlot { get; set; } 
        [field: SerializeField] public float WeaponFireRate { get; set; }
        [field: SerializeField] public int WeaponDurability { get; set; }
        [field: SerializeField] public int AnimationLayer { get; set; }     
        [field: SerializeField] public float BulletMass { get; set; }     
        [field: SerializeField] public float BulletVelocity { get; set; } 
        [field: SerializeField] public int Ammo { get; set; }
        [field: SerializeField] public int MaxAmmo { get; set; }
    }
}