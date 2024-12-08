using System;
using System.Collections.Generic;
using Core.Scripts.Runtime.Items;
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    internal class AgentAmmoPickUp : MonoBehaviour, IItemPickUP<WeaponEnums.WeaponAmmoType>, NotifyEvent
    {
        public event Action NotifyAction;
        
        [SerializeField] private AgentWeaponMotor _weaponMotor;
        
        public void PickUpObject(WeaponEnums.WeaponAmmoType weaponType)
        {
            //Instantiate Ammo Type
        }
    } 
}