using GlobalInputs;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
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
    
    [CreateAssetMenu(menuName = "Core/Create AgentWeapon", fileName = "AgentWeapon")]
    public class WeaponConfigSO : ScriptableObject
    {
        [field: SerializeField] public WeaponType WeaponType { get; private set; }
        [field: SerializeField] public int WeaponInputSlot { get; set; }
        [field: SerializeField] public float WeaponFireRate { get; set; }
        [field: SerializeField] public int WeaponDurability { get; set; }
        [field: SerializeField] public int AnimLayer { get; set; }
        
    }
}