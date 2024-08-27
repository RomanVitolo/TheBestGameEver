using Core.Scripts.Runtime.Weapon;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    public class AgentWeapon : MonoBehaviour
    {
        [Header("Weapon Configuration")]
        [field: SerializeField] public WeaponConfigSO WeaponConfigConfiguration { get; private set; }
    }
}