using System.Collections.Generic;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime
{
    public class AgentWeaponDrop : MonoBehaviour
    {
        public void DropWeapon(List<Weapon> agentWeaponsSlot, Weapon currentWeapon,
            WeaponEnums.WeaponType actualWeaponType, int currentIndex)
        {
            if (agentWeaponsSlot.Count <= 1) return;

            agentWeaponsSlot.Remove(currentWeapon);
            currentWeapon.gameObject.SetActive(false);
            currentWeapon = agentWeaponsSlot[^1];
            if (currentWeapon == null) return;
            actualWeaponType = currentWeapon.WeaponDataConfiguration.WeaponType;
            currentWeapon.gameObject.SetActive(true);
            currentIndex = currentWeapon.WeaponDataConfiguration.WeaponInputSlot;
        }
    }
}