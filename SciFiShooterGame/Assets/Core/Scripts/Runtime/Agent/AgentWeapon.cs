using Core.Scripts.Runtime.Weapon;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    public class AgentWeapon : MonoBehaviour
    {
        [field: SerializeField] public WeaponConfigSO WeaponConfigConfiguration { get; private set; }
    }
}