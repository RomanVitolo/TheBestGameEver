using System.Collections.Generic;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    [CreateAssetMenu(menuName = "Core/Weapon Settings/Create FireModeHolder", fileName = "FireModeHolder")]
    public class WeaponFireModeHolderSO : ScriptableObject
    {
        [field: SerializeField] public List<WeaponFireModeS0> FireModeTypes { get; set; } = new List<WeaponFireModeS0>();
    }
}